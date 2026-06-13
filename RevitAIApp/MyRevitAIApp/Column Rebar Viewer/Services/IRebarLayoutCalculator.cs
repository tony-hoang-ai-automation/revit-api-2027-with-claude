using System.Collections.Generic;
using MyRevitAIApp.ColumnRebarViewer.Models;

namespace MyRevitAIApp.ColumnRebarViewer.Services
{
   /// <summary>
   ///     Tính toán bố trí thép — PURE LOGIC, không tham chiếu Revit/WPF (xUnit-testable).
   /// </summary>
   public interface IRebarLayoutCalculator
   {
      /// <summary>Tham số đai gia cường tự tính theo TCVN (Lo, sConf, sMid).</summary>
      readonly record struct ConfinementResult(double LengthMm, double ConfSpacingMm, double MidSpacingMm);

      /// <summary>
      ///     Tính tâm các thanh thép dọc trên mặt cắt.
      ///     <paramref name="crossTieBarIndices"/> = index các thanh người dùng đã thêm đai phụ chữ C.
      /// </summary>
      RebarSection2D BuildSection(
         double bMm, double hMm, double coverMm, double stirrupDiaMm, double barDiaMm,
         LongitudinalLayout layout, int totalCount, int barsAlongB, int barsAlongH,
         IReadOnlyCollection<int>? crossTieBarIndices = null);

      /// <summary>Tính các lớp đai trên mặt đứng (vùng gia cường 2 đầu + vùng giữa).</summary>
      RebarElevation2D BuildElevation(
         RebarSection2D section, double heightMm,
         double confLengthMm, double confSpacingMm, double midSpacingMm,
         double lneoMm, double lnoiMm);

      /// <summary>Công thức TCVN tính vùng đai gia cường đầu cột.</summary>
      ConfinementResult ComputeConfinement(double bMm, double hMm, double heightMm, int minLongBarDiaMm);

      /// <summary>
      ///     Quy đổi 3 vùng đai (gia cường dưới / giữa / gia cường trên) thành các vùng rải
      ///     Number+Spacing để tạo Rebar trong Revit.
      /// </summary>
      IReadOnlyList<StirrupZone> ComputeStirrupZones(
         double heightMm, double confLengthMm, double confSpacingMm, double midSpacingMm);
   }
}
