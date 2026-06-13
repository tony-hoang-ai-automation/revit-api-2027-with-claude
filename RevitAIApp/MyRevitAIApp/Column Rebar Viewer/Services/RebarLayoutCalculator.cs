using System;
using System.Collections.Generic;
using System.Linq;
using MyRevitAIApp.ColumnRebarViewer.Models;

namespace MyRevitAIApp.ColumnRebarViewer.Services
{
   /// <summary>
   ///     PURE LOGIC tính bố trí thép cột. KHÔNG dùng Autodesk.Revit.* hay System.Windows.*
   ///     để file này share sang test project (xUnit).
   /// </summary>
   public sealed class RebarLayoutCalculator : IRebarLayoutCalculator
   {
      private const int MaxStirrupLines = 5000; // guard cột rất cao

      public RebarSection2D BuildSection(
         double bMm, double hMm, double coverMm, double stirrupDiaMm, double barDiaMm,
         LongitudinalLayout layout, int totalCount, int barsAlongB, int barsAlongH,
         IReadOnlyCollection<int>? crossTieBarIndices = null)
      {
         var empty = new List<BarPoint>();
         var noTies = new List<CrossTie>();

         if (bMm <= 0 || hMm <= 0)
            return new RebarSection2D(bMm, hMm, coverMm, stirrupDiaMm, empty, false, "Kích thước tiết diện không hợp lệ.", noTies);

         // Tâm thép = mép trong cover + đai + nửa thanh
         var inset = coverMm + stirrupDiaMm + barDiaMm / 2.0;
         var x0 = -bMm / 2.0 + inset;
         var x1 = bMm / 2.0 - inset;
         var y0 = -hMm / 2.0 + inset;
         var y1 = hMm / 2.0 - inset;

         if (x1 <= x0 || y1 <= y0)
            return new RebarSection2D(bMm, hMm, coverMm, stirrupDiaMm, empty, false,
               "Lớp bảo vệ/đai quá lớn so với tiết diện — không còn chỗ đặt thép.", noTies);

         var bars = layout == LongitudinalLayout.BySides
            ? PlaceBySides(x0, x1, y0, y1, barsAlongB, barsAlongH, barDiaMm)
            : PlaceByPerimeter(x0, x1, y0, y1, totalCount, barDiaMm);

         if (bars.Count < 4)
            return new RebarSection2D(bMm, hMm, coverMm, stirrupDiaMm, bars, false,
               "Cần tối thiểu 4 thanh thép dọc (mỗi góc 1 thanh).", noTies);

         var ties = BuildCrossTies(bars, crossTieBarIndices, x0, x1, y0, y1);
         var message = CheckCrowding(bars, barDiaMm);
         return new RebarSection2D(bMm, hMm, coverMm, stirrupDiaMm, bars, true, message, ties);
      }

      /// <summary>
      ///     Tính hình học đai phụ chữ C cho từng thanh người dùng đã chọn.
      ///     Xác định cạnh thanh đứng (trên/dưới/trái/phải) → móc sang mặt đối diện cùng trục.
      ///     Thanh góc (nằm trên 2 cạnh) bỏ qua: đã được đai chính ôm.
      /// </summary>
      private static List<CrossTie> BuildCrossTies(
         IReadOnlyList<BarPoint> bars, IReadOnlyCollection<int>? indices,
         double x0, double x1, double y0, double y1)
      {
         var ties = new List<CrossTie>();
         if (indices == null || indices.Count == 0) return ties;

         const double tol = 1.0; // mm — bám mép inset

         foreach (var idx in indices)
         {
            if (idx < 0 || idx >= bars.Count) continue;
            var b = bars[idx];

            var nearLeft   = Math.Abs(b.X - x0) <= tol;
            var nearRight  = Math.Abs(b.X - x1) <= tol;
            var nearBottom = Math.Abs(b.Y - y0) <= tol;
            var nearTop    = Math.Abs(b.Y - y1) <= tol;

            var onVerticalEdge   = nearLeft || nearRight;   // cạnh trái/phải → đai phụ nằm ngang
            var onHorizontalEdge = nearTop  || nearBottom;  // cạnh trên/dưới → đai phụ thẳng đứng

            // Thanh góc (vừa cạnh ngang vừa cạnh dọc) → bỏ qua
            if (onVerticalEdge && onHorizontalEdge) continue;

            if (onHorizontalEdge)
            {
               var y2 = nearTop ? y0 : y1; // móc sang mặt đối diện theo phương đứng
               ties.Add(new CrossTie(b.X, b.Y, b.X, y2, true, idx));
            }
            else if (onVerticalEdge)
            {
               var x2 = nearLeft ? x1 : x0; // móc sang mặt đối diện theo phương ngang
               ties.Add(new CrossTie(b.X, b.Y, x2, b.Y, false, idx));
            }
         }

         return ties;
      }

      private static List<BarPoint> PlaceBySides(double x0, double x1, double y0, double y1, int nB, int nH, double dia)
      {
         var pts = new List<BarPoint>();
         nB = Math.Max(2, nB);
         nH = Math.Max(2, nH);

         // Hàng trên & dưới: nB thanh đều từ x0..x1 (gồm 2 góc)
         for (var i = 0; i < nB; i++)
         {
            var x = nB == 1 ? (x0 + x1) / 2 : x0 + (x1 - x0) * i / (nB - 1);
            pts.Add(new BarPoint(x, y0, dia));
            pts.Add(new BarPoint(x, y1, dia));
         }

         // Cột trái & phải: phần giữa (loại 2 góc đã có) → nH-2 thanh
         for (var j = 1; j < nH - 1; j++)
         {
            var y = y0 + (y1 - y0) * j / (nH - 1);
            pts.Add(new BarPoint(x0, y, dia));
            pts.Add(new BarPoint(x1, y, dia));
         }

         return pts;
      }

      private static List<BarPoint> PlaceByPerimeter(double x0, double x1, double y0, double y1, int n, double dia)
      {
         var pts = new List<BarPoint>();
         if (n < 4) return pts;

         // 4 góc
         pts.Add(new BarPoint(x0, y0, dia));
         pts.Add(new BarPoint(x1, y0, dia));
         pts.Add(new BarPoint(x0, y1, dia));
         pts.Add(new BarPoint(x1, y1, dia));

         var interior = n - 4;
         if (interior <= 0) return pts;

         var wLen = x1 - x0;
         var hLen = y1 - y0;
         var denom = wLen + hLen;

         // Chia thanh giữa cho cặp cạnh ngang (top+bottom) vs cặp cạnh dọc (left+right) theo tỉ lệ chiều dài
         var hPair = denom > 0 ? (int)Math.Round(interior * (wLen / denom)) : interior / 2;
         hPair = Math.Max(0, Math.Min(interior, hPair));
         var vPair = interior - hPair;

         var perTop = hPair / 2;
         var perBottom = hPair - perTop;
         var perLeft = vPair / 2;
         var perRight = vPair - perLeft;

         AddEdge(pts, perTop, k => new BarPoint(x0 + wLen * k, y1, dia));    // cạnh trên
         AddEdge(pts, perBottom, k => new BarPoint(x0 + wLen * k, y0, dia)); // cạnh dưới
         AddEdge(pts, perLeft, k => new BarPoint(x0, y0 + hLen * k, dia));   // cạnh trái
         AddEdge(pts, perRight, k => new BarPoint(x1, y0 + hLen * k, dia));  // cạnh phải

         return pts;
      }

      private static void AddEdge(List<BarPoint> pts, int count, Func<double, BarPoint> factory)
      {
         for (var i = 1; i <= count; i++)
         {
            var k = (double)i / (count + 1); // chia đều, loại 2 đầu (đã là góc)
            pts.Add(factory(k));
         }
      }

      private static string? CheckCrowding(IReadOnlyList<BarPoint> bars, double dia)
      {
         // Khoảng hở thông thuỷ tối thiểu giữa 2 thanh ~ max(25mm, dia) theo TCVN
         var minClear = Math.Max(25.0, dia);
         var minGap = double.MaxValue;
         for (var i = 0; i < bars.Count; i++)
         for (var j = i + 1; j < bars.Count; j++)
         {
            var dx = bars[i].X - bars[j].X;
            var dy = bars[i].Y - bars[j].Y;
            var gap = Math.Sqrt(dx * dx + dy * dy) - dia; // trừ 2 nửa đường kính
            if (gap < minGap) minGap = gap;
         }

         return minGap < minClear
            ? $"Cảnh báo: khoảng hở thép ({minGap:F0}mm) < tối thiểu {minClear:F0}mm — thép quá dày."
            : null;
      }

      public RebarElevation2D BuildElevation(
         RebarSection2D section, double heightMm,
         double confLengthMm, double confSpacingMm, double midSpacingMm,
         double lneoMm, double lnoiMm)
      {
         var barXs = section.Bars
            .Select(b => Math.Round(b.X, 1))
            .Distinct()
            .OrderBy(x => x)
            .ToList();

         var width = section.BMm;

         if (heightMm <= 0 || confSpacingMm <= 0 || midSpacingMm <= 0)
            return new RebarElevation2D(width, heightMm, barXs, new List<StirrupLine>(),
               confLengthMm, confSpacingMm, midSpacingMm, lneoMm, lnoiMm, false,
               "Chiều cao/khoảng cách đai không hợp lệ.");

         var lines = ComputeStirrups(heightMm, confLengthMm, confSpacingMm, midSpacingMm);
         return new RebarElevation2D(width, heightMm, barXs, lines,
            confLengthMm, confSpacingMm, midSpacingMm, lneoMm, lnoiMm, true, null);
      }

      private static List<StirrupLine> ComputeStirrups(double height, double confLen, double sConf, double sMid)
      {
         var lines = new List<StirrupLine>();
         confLen = Math.Max(0, Math.Min(confLen, height / 2.0));

         // Vùng gia cường dưới: 0..confLen
         for (var y = 0.0; y <= confLen + 1e-6 && lines.Count < MaxStirrupLines; y += sConf)
            lines.Add(new StirrupLine(y, true));

         // Vùng giữa: confLen..(height-confLen)
         for (var y = confLen + sMid; y < height - confLen - 1e-6 && lines.Count < MaxStirrupLines; y += sMid)
            lines.Add(new StirrupLine(y, false));

         // Vùng gia cường trên: (height-confLen)..height
         for (var y = height - confLen; y <= height + 1e-6 && lines.Count < MaxStirrupLines; y += sConf)
            lines.Add(new StirrupLine(y, true));

         return lines;
      }

      public IRebarLayoutCalculator.ConfinementResult ComputeConfinement(double bMm, double hMm, double heightMm, int minLongBarDiaMm)
      {
         var maxDim = Math.Max(bMm, hMm);
         var minDim = Math.Min(bMm, hMm);

         // Lo = max(cạnh lớn tiết diện, H/6, 500mm) — TCVN 198:1997 / 5574:2018
         var lo = Math.Max(Math.Max(maxDim, heightMm / 6.0), 500.0);
         lo = Math.Min(lo, heightMm / 2.0); // không vượt nửa cột

         // sConf = min(6*Ø, minDim/4, 100mm) → làm tròn xuống bội 50
         var sConf = RoundDownTo50(Math.Min(Math.Min(6.0 * minLongBarDiaMm, minDim / 4.0), 100.0));

         // sMid = min(15*Ø, minDim, 200mm) → làm tròn xuống bội 50
         var sMid = RoundDownTo50(Math.Min(Math.Min(15.0 * minLongBarDiaMm, minDim), 200.0));

         return new IRebarLayoutCalculator.ConfinementResult(lo, sConf, sMid);
      }

      private static double RoundDownTo50(double v)
      {
         var r = Math.Floor(v / 50.0) * 50.0;
         return Math.Max(50.0, r);
      }

      public IReadOnlyList<StirrupZone> ComputeStirrupZones(
         double heightMm, double confLengthMm, double confSpacingMm, double midSpacingMm)
      {
         var zones = new List<StirrupZone>();
         if (heightMm <= 0) return zones;

         var confLen = Math.Max(0, Math.Min(confLengthMm, heightMm / 2.0));

         // Vùng gia cường dưới: 0 .. confLen
         if (confSpacingMm > 0 && confLen > 0)
         {
            var cb = (int)Math.Floor(confLen / confSpacingMm) + 1;
            zones.Add(new StirrupZone(0, cb, confSpacingMm, true));
         }

         // Vùng giữa: (confLen, heightMm - confLen)
         if (midSpacingMm > 0)
         {
            var midStart = confLen + midSpacingMm;
            var midEnd = heightMm - confLen;
            if (midEnd - midStart > -1e-6)
            {
               var cm = (int)Math.Floor((midEnd - midStart - 1e-6) / midSpacingMm) + 1;
               if (cm > 0) zones.Add(new StirrupZone(midStart, cm, midSpacingMm, false));
            }
         }

         // Vùng gia cường trên: (heightMm - confLen) .. heightMm
         if (confSpacingMm > 0 && confLen > 0)
         {
            var ct = (int)Math.Floor(confLen / confSpacingMm) + 1;
            zones.Add(new StirrupZone(heightMm - confLen, ct, confSpacingMm, true));
         }

         return zones;
      }
   }
}
