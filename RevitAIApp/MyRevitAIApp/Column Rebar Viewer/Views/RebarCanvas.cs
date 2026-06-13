using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MyRevitAIApp.ColumnRebarViewer.Models;
using Color = System.Windows.Media.Color;
using FormattedText = System.Windows.Media.FormattedText;
using Point = System.Windows.Point;

namespace MyRevitAIApp.ColumnRebarViewer.Views
{
   /// <summary>Chế độ vẽ của RebarCanvas.</summary>
   public enum CanvasRenderMode { Section, Elevation }

   /// <summary>
   ///     Custom FrameworkElement vẽ mặt cắt hoặc mặt đứng thép cột lên DrawingContext.
   ///     Zoom/pan bằng mouse wheel + drag. Không dùng ScaleTransform/WPF layout — coordinate
   ///     transform tự tính trong OnRender để kiểm soát hoàn toàn tỉ lệ và text size.
   /// </summary>
   public sealed class RebarCanvas : FrameworkElement
   {
      // ─── Dependency Properties ────────────────────────────────────────────────

      public static readonly DependencyProperty RenderModeProperty =
         DependencyProperty.Register(nameof(RenderMode), typeof(CanvasRenderMode), typeof(RebarCanvas),
            new FrameworkPropertyMetadata(CanvasRenderMode.Section,
               FrameworkPropertyMetadataOptions.AffectsRender));

      public static readonly DependencyProperty SectionModelProperty =
         DependencyProperty.Register(nameof(SectionModel), typeof(RebarSection2D), typeof(RebarCanvas),
            new FrameworkPropertyMetadata(null, OnModelChanged));

      public static readonly DependencyProperty ElevationModelProperty =
         DependencyProperty.Register(nameof(ElevationModel), typeof(RebarElevation2D), typeof(RebarCanvas),
            new FrameworkPropertyMetadata(null, OnModelChanged));

      /// <summary>Command gọi khi click trúng 1 thanh thép dọc (Section mode). Parameter = index thanh.</summary>
      public static readonly DependencyProperty BarClickedCommandProperty =
         DependencyProperty.Register(nameof(BarClickedCommand), typeof(ICommand), typeof(RebarCanvas),
            new FrameworkPropertyMetadata(null));

      public ICommand? BarClickedCommand
      {
         get => (ICommand?)GetValue(BarClickedCommandProperty);
         set => SetValue(BarClickedCommandProperty, value);
      }

      public CanvasRenderMode RenderMode
      {
         get => (CanvasRenderMode)GetValue(RenderModeProperty);
         set => SetValue(RenderModeProperty, value);
      }

      public RebarSection2D? SectionModel
      {
         get => (RebarSection2D?)GetValue(SectionModelProperty);
         set => SetValue(SectionModelProperty, value);
      }

      public RebarElevation2D? ElevationModel
      {
         get => (RebarElevation2D?)GetValue(ElevationModelProperty);
         set => SetValue(ElevationModelProperty, value);
      }

      private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
         => ((RebarCanvas)d).ResetView();

      // ─── Zoom / pan state ─────────────────────────────────────────────────────

      private double _zoom = 1.0;
      private double _panX, _panY;
      private Point _lastDrag;
      private Point _pressPos;
      private bool _isDragging;

      private const double ClickThresholdPx = 4.0; // di chuyển dưới ngưỡng này = click, không phải pan

      private const double ZoomStep   = 1.15;
      private const double ZoomMin    = 0.05;
      private const double ZoomMax    = 40.0;
      private const double MarginPx   = 32.0;

      // ─── Pens / brushes (created once) ───────────────────────────────────────

      private static readonly Pen OutlinePen         = MkPen(Colors.Black, 1.5);
      private static readonly Pen StirrupPen         = MkPen(Colors.DarkGray, 1.0);
      private static readonly Pen ConfinePen         = MkPen(Colors.DarkRed, 1.2);
      private static readonly Pen CrossTiePen        = MkPen(Color.FromRgb(0, 150, 136), 1.6);   // teal — đai phụ chữ C
      private static readonly Pen AnchorRingPen      = MkPen(Color.FromRgb(255, 140, 0), 1.6);    // cam — thanh đã gắn đai phụ
      private static readonly Pen BarPen             = new(Brushes.Transparent, 0) { DashStyle = DashStyles.Solid };
      private static readonly SolidColorBrush BarFill       = Freeze(new SolidColorBrush(Color.FromRgb(50, 50, 180)));
      private static readonly SolidColorBrush CoverBrush    = Freeze(new SolidColorBrush(Color.FromArgb(40, 100, 180, 100)));
      private static readonly SolidColorBrush BackgroundBrush = Freeze(new SolidColorBrush(Color.FromRgb(248, 248, 248)));
      private static readonly Typeface LabelFont = new("Segoe UI");
      private const double LabelFontSize = 11.0;

      private static Pen MkPen(Color c, double t)
      {
         var p = new Pen(new SolidColorBrush(c), t);
         p.Freeze();
         return p;
      }
      private static T Freeze<T>(T v) where T : Freezable { v.Freeze(); return v; }

      // ─── Constructor ──────────────────────────────────────────────────────────

      public RebarCanvas()
      {
         ClipToBounds = true;
         Focusable    = true;
         SizeChanged += (_, _) => InvalidateVisual();

         MouseWheel         += OnMouseWheel;
         MouseLeftButtonDown += OnMouseLeftDown;
         MouseMove          += OnMouseMove;
         MouseLeftButtonUp  += OnMouseLeftUp;
      }

      // ─── Public zoom/fit API (called from code-behind buttons) ────────────────

      public void ZoomIn()  { _zoom = Math.Min(_zoom * ZoomStep, ZoomMax); InvalidateVisual(); }
      public void ZoomOut() { _zoom = Math.Max(_zoom / ZoomStep, ZoomMin); InvalidateVisual(); }
      public void FitToView() { _zoom = 1.0; _panX = 0; _panY = 0; InvalidateVisual(); }

      // ─── Mouse events ─────────────────────────────────────────────────────────

      private void OnMouseWheel(object sender, MouseWheelEventArgs e)
      {
         var mouse   = e.GetPosition(this);
         var factor  = e.Delta > 0 ? ZoomStep : 1.0 / ZoomStep;
         var newZoom = Math.Max(ZoomMin, Math.Min(ZoomMax, _zoom * factor));

         // Zoom toward cursor: keep model-point under cursor
         var mx = (mouse.X - ActualWidth  / 2.0 - _panX) / _zoom;
         var my = (mouse.Y - ActualHeight / 2.0 - _panY) / _zoom;
         _zoom = newZoom;
         _panX = mouse.X - ActualWidth  / 2.0 - mx * _zoom;
         _panY = mouse.Y - ActualHeight / 2.0 - my * _zoom;
         InvalidateVisual();
         e.Handled = true;
      }

      private void OnMouseLeftDown(object sender, MouseButtonEventArgs e)
      {
         _isDragging = true;
         _lastDrag   = e.GetPosition(this);
         _pressPos   = _lastDrag;
         Focus();
         CaptureMouse();
         e.Handled = true;
      }

      private void OnMouseMove(object sender, MouseEventArgs e)
      {
         if (!_isDragging) return;
         var pos = e.GetPosition(this);
         _panX += pos.X - _lastDrag.X;
         _panY += pos.Y - _lastDrag.Y;
         _lastDrag = pos;
         InvalidateVisual();
      }

      private void OnMouseLeftUp(object sender, MouseButtonEventArgs e)
      {
         _isDragging = false;
         ReleaseMouseCapture();

         var pos = e.GetPosition(this);
         var moved = (pos - _pressPos).Length;
         if (moved <= ClickThresholdPx)
            TryClickBar(pos);
      }

      /// <summary>Click (không drag) trên Section → hit-test thanh thép gần nhất → gọi BarClickedCommand.</summary>
      private void TryClickBar(Point screen)
      {
         if (RenderMode != CanvasRenderMode.Section) return;
         var s = SectionModel;
         if (s is not { IsValid: true } || BarClickedCommand == null || _scale <= 0) return;

         var idx = HitTestBar(screen, s);
         if (idx >= 0 && BarClickedCommand.CanExecute(idx))
            BarClickedCommand.Execute(idx);
      }

      /// <summary>Trả về index thanh thép gần con trỏ nhất trong bán kính hit, hoặc -1.</summary>
      private int HitTestBar(Point screen, RebarSection2D s)
      {
         var best = -1;
         var bestDist = double.MaxValue;
         for (var i = 0; i < s.Bars.Count; i++)
         {
            var p = ToScreen(s.Bars[i].X, s.Bars[i].Y);
            var d = (p - screen).Length;
            var hitR = Math.Max(10.0, s.Bars[i].DiameterMm / 2.0 * _scale + 6.0);
            if (d <= hitR && d < bestDist) { bestDist = d; best = i; }
         }
         return best;
      }

      // ─── Coordinate helpers ───────────────────────────────────────────────────

      /// <summary>Model mm → screen px. Model origin is centre of canvas. Y axis flipped.</summary>
      private Point ToScreen(double mx, double my)
         => new(ActualWidth / 2.0 + _panX + mx * _scale,
                ActualHeight / 2.0 + _panY - my * _scale);

      private double _scale; // computed per frame in OnRender

      private void ComputeScale(double modelW, double modelH)
      {
         if (modelW <= 0 || modelH <= 0 || ActualWidth <= 0 || ActualHeight <= 0) { _scale = 1; return; }
         var sx = (ActualWidth  - 2 * MarginPx) / modelW;
         var sy = (ActualHeight - 2 * MarginPx) / modelH;
         _scale = Math.Min(sx, sy) * _zoom;
      }

      // ─── OnRender ─────────────────────────────────────────────────────────────

      protected override void OnRender(DrawingContext dc)
      {
         // background
         dc.DrawRectangle(BackgroundBrush, null, new Rect(0, 0, ActualWidth, ActualHeight));

         if (RenderMode == CanvasRenderMode.Section && SectionModel != null)
            DrawSection(dc, SectionModel);
         else if (RenderMode == CanvasRenderMode.Elevation && ElevationModel != null)
            DrawElevation(dc, ElevationModel);
         else
            DrawPlaceholder(dc, RenderMode == CanvasRenderMode.Section ? "Mặt cắt" : "Mặt đứng");
      }

      // ─── Section drawing ──────────────────────────────────────────────────────

      private void DrawSection(DrawingContext dc, RebarSection2D s)
      {
         ComputeScale(s.BMm + 80, s.HMm + 80);

         // 1. Cover offset (dashed fill)
         var coverRect = new Rect(
            ToScreen(-s.BMm / 2.0 + s.CoverMm, s.HMm / 2.0 - s.CoverMm),
            ToScreen( s.BMm / 2.0 - s.CoverMm, -s.HMm / 2.0 + s.CoverMm));
         var coverPen = new Pen(new SolidColorBrush(Color.FromRgb(100, 180, 100)), 0.7) { DashStyle = DashStyles.Dash };
         dc.DrawRectangle(CoverBrush, coverPen, coverRect);

         // 2. Stirrup inset rounded rect
         var stirIn = s.CoverMm + s.StirrupDiameterMm / 2.0;
         var sRect = new Rect(
            ToScreen(-s.BMm / 2.0 + stirIn,  s.HMm / 2.0 - stirIn),
            ToScreen( s.BMm / 2.0 - stirIn, -s.HMm / 2.0 + stirIn));
         var cornerR = Math.Max(2, s.StirrupDiameterMm * _scale);
         dc.DrawRoundedRectangle(null, StirrupPen, sRect, cornerR, cornerR);

         // 2b. Móc 135° đai kín tại góc trên-trái (thể hiện đai thật)
         DrawStirrupHook(dc, -s.BMm / 2.0 + stirIn, s.HMm / 2.0 - stirIn);

         // 3. Column outline
         var colRect = new Rect(ToScreen(-s.BMm / 2.0, s.HMm / 2.0), ToScreen(s.BMm / 2.0, -s.HMm / 2.0));
         dc.DrawRectangle(null, OutlinePen, colRect);

         // 4. Đai phụ chữ C (vẽ dưới thanh để thanh nổi lên trên)
         if (s.IsValid)
            foreach (var tie in s.CrossTies)
               DrawCrossTie(dc, tie);

         // 5. Bars (+ ring cam đánh dấu thanh đã gắn đai phụ)
         if (s.IsValid)
         {
            foreach (var bar in s.Bars)
            {
               var r = Math.Max(2, bar.DiameterMm / 2.0 * _scale);
               dc.DrawEllipse(BarFill, null, ToScreen(bar.X, bar.Y), r, r);
            }

            foreach (var tie in s.CrossTies)
            {
               var c = ToScreen(tie.X1, tie.Y1);
               var rr = Math.Max(5, 5 + _scale * 6);
               dc.DrawEllipse(null, AnchorRingPen, c, rr, rr);
            }
         }

         // 6. Dimension labels b / h
         DrawLabel(dc, $"b = {s.BMm:F0} mm", ToScreen(0, -s.HMm / 2.0 - 18 / _scale), true);
         DrawLabel(dc, $"h = {s.HMm:F0} mm", ToScreen(s.BMm / 2.0 + 18 / _scale, 0), false);

         // 7. Status / warning
         if (!string.IsNullOrEmpty(s.Message))
            DrawLabel(dc, s.Message, new Point(8, ActualHeight - 18), true, Colors.DarkRed);
      }

      /// <summary>Vẽ 1 đai phụ chữ C: chân thẳng + 2 móc 135° ở hai đầu.</summary>
      private void DrawCrossTie(DrawingContext dc, CrossTie tie)
      {
         var p1 = ToScreen(tie.X1, tie.Y1);
         var p2 = ToScreen(tie.X2, tie.Y2);
         var dx = p2.X - p1.X;
         var dy = p2.Y - p1.Y;
         var len = Math.Sqrt(dx * dx + dy * dy);
         if (len < 1) return;

         dc.DrawLine(CrossTiePen, p1, p2);

         var ux = dx / len; var uy = dy / len;   // dọc chân đai
         var vx = -uy; var vy = ux;              // vuông góc chân
         var hk = Math.Max(7.0, 22.0 * _scale);

         DrawCTieHook(dc, p1, ux, uy, vx, vy, hk);   // móc đầu neo (hướng vào lõi)
         DrawCTieHook(dc, p2, -ux, -uy, vx, vy, hk); // móc đầu kia
      }

      private static void DrawCTieHook(DrawingContext dc, Point end,
         double inX, double inY, double vx, double vy, double hk)
      {
         // thanh ngang của móc (ôm thanh thép)
         var a = new Point(end.X + vx * hk * 0.5, end.Y + vy * hk * 0.5);
         var b = new Point(end.X - vx * hk * 0.5, end.Y - vy * hk * 0.5);
         dc.DrawLine(CrossTiePen, a, b);
         // đoạn bẻ 135° quay vào lõi
         var r = new Point(a.X + inX * hk * 0.7, a.Y + inY * hk * 0.7);
         dc.DrawLine(CrossTiePen, a, r);
      }

      /// <summary>Móc 135° đai kín tại 1 góc — đoạn chéo ngắn quay vào lõi.</summary>
      private void DrawStirrupHook(DrawingContext dc, double cornerXmm, double cornerYmm)
      {
         var c = ToScreen(cornerXmm, cornerYmm);
         var hk = Math.Max(6.0, 16.0 * _scale);
         dc.DrawLine(StirrupPen, c, new Point(c.X + hk, c.Y + hk)); // vào lõi (xuống-phải)
      }

      // ─── Elevation drawing ────────────────────────────────────────────────────

      private void DrawElevation(DrawingContext dc, RebarElevation2D e)
      {
         ComputeScale(e.WidthMm + 100, e.HeightMm + 100);

         var w = e.WidthMm;
         var h = e.HeightMm;

         // 1. Column outline (origin = bottom-centre)
         var tl = ToScreen(-w / 2.0, h);
         var br = ToScreen( w / 2.0, 0);
         dc.DrawRectangle(null, OutlinePen, new Rect(tl, br));

         // 2. Longitudinal bars (vertical lines)
         foreach (var x in e.BarXs)
            dc.DrawLine(new Pen(BarFill, Math.Max(1.5, BarFill.Color.A / 255.0 * 3)),
               ToScreen(x, 0), ToScreen(x, h));

         // 3. Stirrups
         foreach (var sl in e.Stirrups)
         {
            var pen = sl.IsConfinement ? ConfinePen : StirrupPen;
            dc.DrawLine(pen, ToScreen(-w / 2.0, sl.Y), ToScreen(w / 2.0, sl.Y));
         }

         // 4. Spacing labels (centre of each zone)
         var mid = h / 2.0;
         if (e.ConfinementLengthMm > 0 && e.ConfinementSpacingMm > 0)
            DrawLabel(dc, $"a{e.ConfinementSpacingMm:F0}", ToScreen(w / 2.0 + 5 / _scale, e.ConfinementLengthMm / 2.0), false, Colors.DarkRed);
         if (e.MidSpacingMm > 0)
            DrawLabel(dc, $"a{e.MidSpacingMm:F0}", ToScreen(w / 2.0 + 5 / _scale, mid), false);
         if (e.ConfinementLengthMm > 0 && e.ConfinementSpacingMm > 0)
            DrawLabel(dc, $"a{e.ConfinementSpacingMm:F0}", ToScreen(w / 2.0 + 5 / _scale, h - e.ConfinementLengthMm / 2.0), false, Colors.DarkRed);

         // 5. Lneo / Lnối markers (dashed horizontal line + label on left)
         DrawHMarker(dc, e.LneoMm, -w / 2.0 - 30 / _scale, $"Lneo={e.LneoMm:F0}", Colors.DarkSlateBlue);
         DrawHMarker(dc, e.LnoiMm, -w / 2.0 - 30 / _scale, $"Lnối={e.LnoiMm:F0}", Colors.DarkGoldenrod);

         // 6. Height label
         DrawLabel(dc, $"H = {h:F0} mm", ToScreen(0, -20 / _scale), true);
      }

      private void DrawHMarker(DrawingContext dc, double y, double labelX, string text, Color col)
      {
         if (y <= 0) return;
         var dashedPen = new Pen(new SolidColorBrush(col), 0.8) { DashStyle = DashStyles.Dash };
         dc.DrawLine(dashedPen, ToScreen(labelX, y), ToScreen(0, y));
         DrawLabel(dc, text, ToScreen(labelX, y), false, col);
      }

      // ─── Label helper ─────────────────────────────────────────────────────────

      private void DrawLabel(DrawingContext dc, string text, Point pos, bool centreX,
         Color? col = null)
      {
         var ft = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
            LabelFont, LabelFontSize, new SolidColorBrush(col ?? Colors.DimGray),
            VisualTreeHelper.GetDpi(this).PixelsPerDip);
         if (centreX) pos.X -= ft.Width / 2;
         dc.DrawText(ft, pos);
      }

      private void DrawPlaceholder(DrawingContext dc, string name)
      {
         DrawLabel(dc, $"[{name}]", new Point(ActualWidth / 2.0, ActualHeight / 2.0), true);
      }

      private void ResetView() { _zoom = 1.0; _panX = 0; _panY = 0; InvalidateVisual(); }
   }
}
