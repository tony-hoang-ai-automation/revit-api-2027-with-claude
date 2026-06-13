using MyRevitAIApp.ColumnRebarViewer.Models;
using MyRevitAIApp.ColumnRebarViewer.Services;
using Xunit;

namespace MyRevitAIApp.Tests.ColumnRebarViewer
{
   public sealed class RebarLayoutCalculatorTests
   {
      private readonly RebarLayoutCalculator _calc = new();

      // ─── BuildSection ────────────────────────────────────────────────────────

      [Fact]
      public void BuildSection_ByPerimeter_ReturnsTotalBars()
      {
         var sec = _calc.BuildSection(400, 400, 30, 8, 20,
            LongitudinalLayout.ByPerimeter, 8, 0, 0);
         Assert.True(sec.IsValid);
         Assert.Equal(8, sec.Bars.Count);
      }

      [Fact]
      public void BuildSection_ByPerimeter_AllBarsInsideSection()
      {
         const double b = 400, h = 600, cover = 30, stirDia = 8, barDia = 20;
         var sec = _calc.BuildSection(b, h, cover, stirDia, barDia,
            LongitudinalLayout.ByPerimeter, 12, 0, 0);
         Assert.True(sec.IsValid);
         var halfB = b / 2.0;
         var halfH = h / 2.0;
         foreach (var bar in sec.Bars)
         {
            Assert.True(Math.Abs(bar.X) <= halfB, $"Bar X={bar.X} outside b/2={halfB}");
            Assert.True(Math.Abs(bar.Y) <= halfH, $"Bar Y={bar.Y} outside h/2={halfH}");
         }
      }

      [Fact]
      public void BuildSection_ByPerimeter_4Bars_HasFourCorners()
      {
         var sec = _calc.BuildSection(300, 500, 30, 8, 20,
            LongitudinalLayout.ByPerimeter, 4, 0, 0);
         Assert.True(sec.IsValid);
         Assert.Equal(4, sec.Bars.Count);
      }

      [Fact]
      public void BuildSection_BySides_DeduplicatesCorners()
      {
         // nB=3, nH=3 → 2*3 + 2*3 - 4 = 8 bars
         var sec = _calc.BuildSection(400, 600, 30, 8, 20,
            LongitudinalLayout.BySides, 0, 3, 3);
         Assert.True(sec.IsValid);
         Assert.Equal(8, sec.Bars.Count);
      }

      [Fact]
      public void BuildSection_BySides_nB2nH2_Returns4Bars()
      {
         // Minimum: 2 per side = 4 total (corners only)
         var sec = _calc.BuildSection(400, 400, 30, 8, 20,
            LongitudinalLayout.BySides, 0, 2, 2);
         Assert.True(sec.IsValid);
         Assert.Equal(4, sec.Bars.Count);
      }

      [Fact]
      public void BuildSection_CoverTooLarge_ReturnsInvalid()
      {
         // cover 200 on a 300mm section — no room
         var sec = _calc.BuildSection(300, 300, 200, 8, 20,
            LongitudinalLayout.ByPerimeter, 8, 0, 0);
         Assert.False(sec.IsValid);
         Assert.Empty(sec.Bars);
      }

      [Fact]
      public void BuildSection_NegativeDimensions_ReturnsInvalid()
      {
         var sec = _calc.BuildSection(-100, 400, 30, 8, 20,
            LongitudinalLayout.ByPerimeter, 8, 0, 0);
         Assert.False(sec.IsValid);
      }

      [Fact]
      public void BuildSection_LessThan4Bars_ReturnsInvalid()
      {
         var sec = _calc.BuildSection(400, 400, 30, 8, 20,
            LongitudinalLayout.ByPerimeter, 3, 0, 0);
         Assert.False(sec.IsValid);
      }

      [Fact]
      public void BuildSection_BarDiameter_CorrectInOutput()
      {
         var sec = _calc.BuildSection(400, 400, 30, 8, 25,
            LongitudinalLayout.ByPerimeter, 4, 0, 0);
         Assert.True(sec.IsValid);
         foreach (var bar in sec.Bars)
            Assert.Equal(25, bar.DiameterMm);
      }

      // ─── CrossTie (đai phụ chữ C) ────────────────────────────────────────────

      [Fact]
      public void BuildSection_NoCrossTieIndices_ReturnsEmptyTies()
      {
         var sec = _calc.BuildSection(400, 600, 30, 8, 20,
            LongitudinalLayout.ByPerimeter, 8, 0, 0);
         Assert.Empty(sec.CrossTies);
      }

      [Fact]
      public void BuildSection_EdgeBar_ProducesOneCrossTie()
      {
         // n=8 ByPerimeter: index 0..3 là 4 góc, index ≥4 là thanh giữa cạnh (không góc).
         var sec = _calc.BuildSection(400, 600, 30, 8, 20,
            LongitudinalLayout.ByPerimeter, 8, 0, 0, new[] { 4 });
         Assert.Single(sec.CrossTies);
         Assert.Equal(4, sec.CrossTies[0].AnchorBarIndex);
      }

      [Fact]
      public void BuildSection_CornerBar_ProducesNoCrossTie()
      {
         // index 0 là thanh góc → bị bỏ qua (đã có đai chính ôm).
         var sec = _calc.BuildSection(400, 600, 30, 8, 20,
            LongitudinalLayout.ByPerimeter, 8, 0, 0, new[] { 0 });
         Assert.Empty(sec.CrossTies);
      }

      [Fact]
      public void BuildSection_CrossTie_EndpointOnOppositeFace()
      {
         var sec = _calc.BuildSection(400, 600, 30, 8, 20,
            LongitudinalLayout.ByPerimeter, 8, 0, 0, new[] { 4 });
         var tie = sec.CrossTies[0];
         var anchor = sec.Bars[tie.AnchorBarIndex];
         // Điểm neo phải trùng tâm thanh được chọn.
         Assert.Equal(anchor.X, tie.X1, 3);
         Assert.Equal(anchor.Y, tie.Y1, 3);
         // Đầu kia nằm trên trục đối diện: cùng X (đai đứng) hoặc cùng Y (đai ngang).
         var sameX = Math.Abs(tie.X1 - tie.X2) < 1e-6;
         var sameY = Math.Abs(tie.Y1 - tie.Y2) < 1e-6;
         Assert.True(sameX ^ sameY, "Đai phụ phải thẳng theo đúng 1 phương.");
      }

      [Fact]
      public void BuildSection_OutOfRangeIndex_IgnoredSafely()
      {
         var sec = _calc.BuildSection(400, 600, 30, 8, 20,
            LongitudinalLayout.ByPerimeter, 8, 0, 0, new[] { 999, -5 });
         Assert.Empty(sec.CrossTies);
      }

      // ─── BuildElevation / ComputeStirrups ────────────────────────────────────

      [Fact]
      public void BuildElevation_HasStirrupsAtBottom()
      {
         var sec = _calc.BuildSection(400, 400, 30, 8, 20,
            LongitudinalLayout.ByPerimeter, 8, 0, 0);
         var elev = _calc.BuildElevation(sec, 3000, 500, 100, 200, 600, 700);

         Assert.True(elev.IsValid);
         Assert.Contains(elev.Stirrups, s => s.Y == 0 && s.IsConfinement);
      }

      [Fact]
      public void BuildElevation_HasStirrupsAtTop()
      {
         var sec = _calc.BuildSection(400, 400, 30, 8, 20,
            LongitudinalLayout.ByPerimeter, 8, 0, 0);
         var elev = _calc.BuildElevation(sec, 3000, 500, 100, 200, 600, 700);

         Assert.True(elev.IsValid);
         Assert.Contains(elev.Stirrups, s => s.Y >= 3000 - 1e-6 && s.IsConfinement);
      }

      [Fact]
      public void BuildElevation_MidZoneStirrups_NotConfinement()
      {
         var sec = _calc.BuildSection(400, 400, 30, 8, 20,
            LongitudinalLayout.ByPerimeter, 8, 0, 0);
         var elev = _calc.BuildElevation(sec, 3000, 500, 100, 200, 600, 700);

         Assert.True(elev.IsValid);
         var mid = elev.Stirrups.Where(s => s.Y > 500 + 1e-6 && s.Y < 2500 - 1e-6).ToList();
         Assert.NotEmpty(mid);
         Assert.All(mid, s => Assert.False(s.IsConfinement));
      }

      [Fact]
      public void BuildElevation_ZeroHeight_ReturnsInvalid()
      {
         var sec = _calc.BuildSection(400, 400, 30, 8, 20,
            LongitudinalLayout.ByPerimeter, 8, 0, 0);
         var elev = _calc.BuildElevation(sec, 0, 500, 100, 200, 600, 700);
         Assert.False(elev.IsValid);
      }

      [Fact]
      public void BuildElevation_ZeroSpacing_ReturnsInvalid()
      {
         var sec = _calc.BuildSection(400, 400, 30, 8, 20,
            LongitudinalLayout.ByPerimeter, 8, 0, 0);
         var elev = _calc.BuildElevation(sec, 3000, 500, 0, 200, 600, 700);
         Assert.False(elev.IsValid);
      }

      // ─── ComputeStirrupZones (rải đai cho Rebar) ─────────────────────────────

      [Fact]
      public void ComputeStirrupZones_HasBottomMidTop()
      {
         var zones = _calc.ComputeStirrupZones(3000, 500, 100, 200);
         Assert.Equal(3, zones.Count);
         Assert.True(zones[0].IsConfinement);   // dưới
         Assert.False(zones[1].IsConfinement);  // giữa
         Assert.True(zones[2].IsConfinement);   // trên
      }

      [Fact]
      public void ComputeStirrupZones_BottomStartsAtZero()
      {
         var zones = _calc.ComputeStirrupZones(3000, 500, 100, 200);
         Assert.Equal(0, zones[0].FirstZMm, 3);
         Assert.Equal(100, zones[0].SpacingMm, 3);
         Assert.True(zones[0].Count >= 2);
      }

      [Fact]
      public void ComputeStirrupZones_TopZoneReachesNearTop()
      {
         var zones = _calc.ComputeStirrupZones(3000, 500, 100, 200);
         var top = zones[^1];
         var lastZ = top.FirstZMm + (top.Count - 1) * top.SpacingMm;
         Assert.True(lastZ >= 3000 - top.SpacingMm, $"Đai trên cùng z={lastZ} phải gần đỉnh 3000.");
      }

      [Fact]
      public void ComputeStirrupZones_ZeroHeight_ReturnsEmpty()
      {
         Assert.Empty(_calc.ComputeStirrupZones(0, 500, 100, 200));
      }

      [Fact]
      public void ComputeStirrupZones_ShortColumn_ConfClampedToHalf()
      {
         // confLen 2000 > height/2=500 → bị kẹp; vẫn ra vùng hợp lệ, không âm count.
         var zones = _calc.ComputeStirrupZones(1000, 2000, 100, 200);
         Assert.NotEmpty(zones);
         Assert.All(zones, z => Assert.True(z.Count >= 1));
      }

      // ─── ComputeConfinement (TCVN formula) ───────────────────────────────────

      [Fact]
      public void ComputeConfinement_Lo_AtLeast500mm()
      {
         var r = _calc.ComputeConfinement(200, 200, 2800, 16);
         Assert.True(r.LengthMm >= 500, $"Lo={r.LengthMm} should be ≥ 500mm");
      }

      [Fact]
      public void ComputeConfinement_Lo_AtLeastMaxSectionDim()
      {
         // b=500, h=700 → Lo ≥ 700
         var r = _calc.ComputeConfinement(500, 700, 4000, 20);
         Assert.True(r.LengthMm >= 700, $"Lo={r.LengthMm} should be ≥ max(b,h)=700");
      }

      [Fact]
      public void ComputeConfinement_ConfSpacing_AtMost100()
      {
         var r = _calc.ComputeConfinement(300, 300, 3000, 20);
         Assert.True(r.ConfSpacingMm <= 100, $"sConf={r.ConfSpacingMm} should be ≤ 100mm");
      }

      [Fact]
      public void ComputeConfinement_MidSpacing_AtMost200()
      {
         var r = _calc.ComputeConfinement(300, 400, 3000, 20);
         Assert.True(r.MidSpacingMm <= 200, $"sMid={r.MidSpacingMm} should be ≤ 200mm");
      }

      [Fact]
      public void ComputeConfinement_SpacingMultipleOf50()
      {
         var r = _calc.ComputeConfinement(300, 500, 3500, 20);
         Assert.Equal(0, (int)r.ConfSpacingMm % 50);
         Assert.Equal(0, (int)r.MidSpacingMm % 50);
      }

      [Fact]
      public void ComputeConfinement_Lo_NotExceedHalfHeight()
      {
         // Cột ngắn 1000mm → Lo ≤ 500mm
         var r = _calc.ComputeConfinement(400, 400, 1000, 20);
         Assert.True(r.LengthMm <= 500, $"Lo={r.LengthMm} should be ≤ H/2=500mm");
      }
   }
}
