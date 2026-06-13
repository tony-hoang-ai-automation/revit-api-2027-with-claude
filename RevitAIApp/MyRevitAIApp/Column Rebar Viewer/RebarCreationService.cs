using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB.Structure;
using MyRevitAIApp.ColumnRebarViewer.Models;
using Serilog;

namespace MyRevitAIApp.ColumnRebarViewer
{
   /// <summary>
   ///     Tạo Rebar THẬT vào model Revit cho 1 cột chữ nhật: thép dọc + đai kín + đai phụ chữ C.
   ///     CHẠM Revit API — không share sang test project.
   ///     Gọi trong context API hợp lệ (modal dialog của ExternalCommand) nên tự mở Transaction.
   /// </summary>
   public sealed class RebarCreationService
   {
      private const double FeetPerMm = 1.0 / 304.8;
      private readonly ILogger _logger = Log.ForContext<RebarCreationService>();

      /// <summary>
      ///     Tạo toàn bộ thép theo <paramref name="req"/> vào <paramref name="column"/>.
      ///     Mỗi phần tử tạo trong 1 SubTransaction riêng nên 1 thanh lỗi không làm hỏng cả bộ —
      ///     trả về số lượng tạo được từng loại + lỗi đầu tiên để pinpoint stage hỏng.
      ///     Ném exception cho các lỗi tiền điều kiện (thiếu type, input sai).
      /// </summary>
      public RebarCreationResult Create(FamilyInstance column, RebarCreationRequest req)
      {
         if (column == null) throw new ArgumentNullException(nameof(column));
         if (req.Bars.Count < 4) throw new InvalidOperationException("Cần tối thiểu 4 thanh thép dọc.");
         if (req.HeightMm <= 0) throw new InvalidOperationException("Chiều cao cột không hợp lệ.");

         var doc = column.Document;

         var longType    = FindBarType(doc, req.LongBarDiameterMm);
         var stirrupType = FindBarType(doc, req.StirrupDiameterMm);
         var hookType    = FindHookType(doc); // có thể null → đai không móc

         var tf = column.GetTransform();
         var basisX = tf.BasisX;
         var basisY = tf.BasisY;
         var basisZ = tf.BasisZ;
         var origin = tf.Origin;

         // mm (toạ độ tiết diện) → world XYZ tại cao độ z (mm tính từ chân cột)
         XYZ Pt(double xMm, double yMm, double zMm) =>
            origin
            + basisX.Multiply(xMm * FeetPerMm)
            + basisY.Multiply(yMm * FeetPerMm)
            + basisZ.Multiply(zMm * FeetPerMm);

         var inset = req.CoverMm + req.StirrupDiameterMm / 2.0;
         var hx = req.BMm / 2.0 - inset;
         var hy = req.HMm / 2.0 - inset;
         if (hx <= 0 || hy <= 0)
            throw new InvalidOperationException("Cover/đai quá lớn — không còn chỗ đặt đai.");

         int longC = 0, stirC = 0, tieC = 0, fails = 0;
         string? firstError = null;

         // Tạo 1 phần tử trong SubTransaction riêng; lỗi được cô lập + ghi log.
         bool TryMake(string what, System.Func<Rebar?> make)
         {
            using var st = new SubTransaction(doc);
            st.Start();
            try
            {
               make();
               st.Commit();
               return true;
            }
            catch (Exception ex)
            {
               st.RollBack();
               fails++;
               firstError ??= $"{what}: {ex.Message}";
               _logger.Error(ex, "Tạo {What} thất bại", what);
               return false;
            }
         }

         using var t = new Transaction(doc, "Tạo thép cột");
         t.Start();

         // 1. Thép dọc (Standard, suốt chiều cao). normal ⟂ trục thanh = basisX.
         foreach (var bar in req.Bars)
            if (TryMake("thép dọc", () =>
                {
                   var line = Line.CreateBound(Pt(bar.X, bar.Y, 0), Pt(bar.X, bar.Y, req.HeightMm));
                   return CreateBars(doc, RebarStyle.Standard, longType, null, column, basisX,
                      new List<Curve> { line });
                }))
               longC++;

         // 2. Đai kín (StirrupTie, vòng chữ nhật ngang), rải theo từng vùng. normal = trục cột.
         foreach (var zone in req.Zones)
            if (TryMake("đai kín", () =>
                {
                   var z = zone.FirstZMm;
                   var loop = new List<Curve>
                   {
                      Line.CreateBound(Pt(-hx, -hy, z), Pt( hx, -hy, z)),
                      Line.CreateBound(Pt( hx, -hy, z), Pt( hx,  hy, z)),
                      Line.CreateBound(Pt( hx,  hy, z), Pt(-hx,  hy, z)),
                      Line.CreateBound(Pt(-hx,  hy, z), Pt(-hx, -hy, z)),
                   };
                   var r = CreateBars(doc, RebarStyle.StirrupTie, stirrupType, hookType, column, basisZ, loop);
                   ApplyZoneLayout(r, zone);
                   return r;
                }))
               stirC++;

         // 3. Đai phụ chữ C (Standard 1 thanh ngang + móc), rải như đai kín.
         foreach (var tie in req.CrossTies)
         foreach (var zone in req.Zones)
            if (TryMake("đai phụ chữ C", () =>
                {
                   var z = zone.FirstZMm;
                   var line = Line.CreateBound(Pt(tie.X1, tie.Y1, z), Pt(tie.X2, tie.Y2, z));
                   var r = CreateBars(doc, RebarStyle.Standard, stirrupType, hookType, column, basisZ,
                      new List<Curve> { line });
                   ApplyZoneLayout(r, zone);
                   return r;
                }))
               tieC++;

         if (longC + stirC + tieC == 0)
         {
            t.RollBack();
            _logger.Warning("Không tạo được phần tử thép nào. Lỗi đầu: {Err}", firstError);
         }
         else
         {
            t.Commit();
            _logger.Information("Tạo thép cột {Id}: {Long} dọc, {Stir} đai kín, {Tie} đai phụ; {Fail} lỗi.",
               column.Id, longC, stirC, tieC, fails);
         }

         return new RebarCreationResult(longC, stirC, tieC, fails, firstError);
      }

      private static void ApplyZoneLayout(Rebar? rebar, StirrupZone zone)
      {
         if (rebar == null || zone.Count <= 1) return; // 1 vị trí: giữ nguyên, không cần layout
         if (!rebar.IsRebarShapeDriven()) return;        // free-form: không có shape-driven accessor
         var acc = rebar.GetShapeDrivenAccessor();
         // (numberOfBarPositions, spacing[ft], barsOnNormalSide, includeFirstBar, includeLastBar)
         acc.SetLayoutAsNumberWithSpacing(zone.Count, zone.SpacingMm * FeetPerMm, true, true, true);
      }

      /// <summary>
      ///     Bọc Rebar.CreateFromCurves theo version: R26+ dùng BarTerminationsData,
      ///     R23–R25 dùng RebarHookType + RebarHookOrientation (đã bị bỏ ở R27).
      /// </summary>
      private static Rebar CreateBars(
         Document doc, RebarStyle style, RebarBarType barType, RebarHookType? hookType,
         Element host, XYZ normal, IList<Curve> curves)
      {
#if REVIT2026_OR_GREATER
         var term = new BarTerminationsData(doc);
         if (hookType != null)
         {
            term.HookTypeIdAtStart = hookType.Id;
            term.HookTypeIdAtEnd   = hookType.Id;
         }
         return Rebar.CreateFromCurves(doc, style, barType, host, normal, curves, term, true, true);
#else
         return Rebar.CreateFromCurves(
            doc, style, barType, hookType, hookType, host, normal, curves,
            RebarHookOrientation.Right, RebarHookOrientation.Right, true, true);
#endif
      }

      // ─── Tra cứu type ─────────────────────────────────────────────────────────

      private static RebarBarType FindBarType(Document doc, int diaMm)
      {
         var types = new FilteredElementCollector(doc)
            .OfClass(typeof(RebarBarType))
            .Cast<RebarBarType>()
            .ToList();
         if (types.Count == 0)
            throw new InvalidOperationException(
               "Project chưa có RebarBarType nào. Hãy load 1 loại thép (Rebar Bar) vào project trước.");

         var diaFt = diaMm * FeetPerMm;
         return types.OrderBy(bt => Math.Abs(bt.BarModelDiameter - diaFt)).First();
      }

      private static RebarHookType? FindHookType(Document doc) =>
         new FilteredElementCollector(doc)
            .OfClass(typeof(RebarHookType))
            .Cast<RebarHookType>()
            .FirstOrDefault();
   }
}
