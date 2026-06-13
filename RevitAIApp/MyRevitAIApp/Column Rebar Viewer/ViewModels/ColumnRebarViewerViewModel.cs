using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyRevitAIApp.ColumnRebarViewer.Models;
using MyRevitAIApp.ColumnRebarViewer.Services;
using Serilog;

namespace MyRevitAIApp.ColumnRebarViewer.ViewModels
{
   /// <summary>
   ///     ViewModel cho dialog "Tạo New PF" — vẽ thép cột.
   ///     Mọi thay đổi input tự trigger Recompute() → cập nhật Section + Elevation → Canvas redraw.
   /// </summary>
   public sealed partial class ColumnRebarViewerViewModel : ObservableObject
   {
      private readonly IRebarLayoutCalculator _calculator = new RebarLayoutCalculator();
      private readonly IRebarConfigStore _store = new RebarConfigStore();
      private readonly ILogger _logger = Log.ForContext<ColumnRebarViewerViewModel>();

      /// <summary>Index các thanh thép dọc người dùng đã gắn đai phụ chữ C (click trên mặt cắt).</summary>
      private readonly HashSet<int> _crossTieBars = new();

      /// <summary>Callback ghi Rebar vào model (Command inject; null = chế độ chỉ xem / design-time).</summary>
      private readonly Func<RebarCreationRequest, RebarCreationResult>? _createRebarAction;

      // ─── Geometry (đọc từ Revit, không sửa được) ───────────────────────────
      [ObservableProperty] private double _sectionB;
      [ObservableProperty] private double _sectionH;
      [ObservableProperty] private double _columnHeight;

      // ─── Thép dọc chủ ───────────────────────────────────────────────────────
      [ObservableProperty] private int _longBarDiameter = 20;
      [ObservableProperty] private int _longBarCount = 8;
      [ObservableProperty] private LongitudinalLayout _selectedLayout = LongitudinalLayout.ByPerimeter;
      [ObservableProperty] private int _barsAlongB = 3;
      [ObservableProperty] private int _barsAlongH = 3;

      // ─── Thép đai ───────────────────────────────────────────────────────────
      [ObservableProperty] private int _stirrupDiameter = 8;
      [ObservableProperty] private double _stirrupSpacing = 200;
      [ObservableProperty] private double _confinementSpacing = 100;
      [ObservableProperty] private double _confinementLength = 500;
      [ObservableProperty] private bool _autoTcvn = true;

      // ─── Vật liệu + bảo vệ ──────────────────────────────────────────────────
      [ObservableProperty] private double _coverMm = 30;
      [ObservableProperty] private ConcreteGrade _selectedConcrete = ConcreteGrade.B25;
      [ObservableProperty] private SteelGroup _selectedSteel = SteelGroup.CB400;

      // ─── Neo / nối thép ─────────────────────────────────────────────────────
      [ObservableProperty] private double _lneoMm = 600;
      [ObservableProperty] private double _lnoiMm = 700;

      // ─── Trạng thái / status ─────────────────────────────────────────────────
      [ObservableProperty] private string _statusMessage = string.Empty;
      [ObservableProperty] private bool _isSidesBased;

      // ─── Output cho Canvas ───────────────────────────────────────────────────
      [ObservableProperty] private RebarSection2D? _section;
      [ObservableProperty] private RebarElevation2D? _elevation;

      // ─── Catalogs cho ComboBox ───────────────────────────────────────────────
      public int[] LongBarDiameters { get; } = BarCatalog.LongitudinalDiameters;
      public int[] StirrupDiameters { get; } = BarCatalog.StirrupDiameters;
      public ObservableCollection<LongitudinalLayout> Layouts { get; } =
         new([LongitudinalLayout.ByPerimeter, LongitudinalLayout.BySides]);
      public ObservableCollection<ConcreteGrade> ConcreteGrades { get; } =
         new((ConcreteGrade[])Enum.GetValues(typeof(ConcreteGrade)));
      public ObservableCollection<SteelGroup> SteelGroups { get; } =
         new((SteelGroup[])Enum.GetValues(typeof(SteelGroup)));

      // ────────────────────────────────────────────────────────────────────────

      public ColumnRebarViewerViewModel(ColumnGeometry geometry,
         Func<RebarCreationRequest, RebarCreationResult>? createRebarAction = null)
      {
         _createRebarAction = createRebarAction;
         _sectionB = geometry.BMm;
         _sectionH = geometry.HMm;
         _columnHeight = geometry.HeightMm;
         AutoFillConfinement();
         Recompute();
      }

      /// <summary>Chỉ tạo được thép khi đang chạy trong Revit (có callback).</summary>
      public bool CanCreateRebar => _createRebarAction != null;

      // ─── Partial onChange: mọi input thay đổi → Recompute ───────────────────

      // Các thay đổi làm dịch chuyển/đánh lại index thanh dọc → đai phụ chữ C cũ không còn đúng → clear.
      partial void OnLongBarDiameterChanged(int value)    => ClearCrossTiesAndRecompute();
      partial void OnLongBarCountChanged(int value)       => ClearCrossTiesAndRecompute();
      partial void OnSelectedLayoutChanged(LongitudinalLayout value)
      {
         IsSidesBased = value == LongitudinalLayout.BySides;
         ClearCrossTiesAndRecompute();
      }
      partial void OnBarsAlongBChanged(int value)         => ClearCrossTiesAndRecompute();
      partial void OnBarsAlongHChanged(int value)         => ClearCrossTiesAndRecompute();
      partial void OnCoverMmChanged(double value)         => ClearCrossTiesAndRecompute();

      // Các thay đổi không ảnh hưởng vị trí thanh dọc → giữ nguyên đai phụ.
      partial void OnStirrupDiameterChanged(int value)    => RebuildConfinementThenRecompute();
      partial void OnStirrupSpacingChanged(double value)  => Recompute();
      partial void OnConfinementSpacingChanged(double value) => Recompute();
      partial void OnConfinementLengthChanged(double value)  => Recompute();
      partial void OnAutoTcvnChanged(bool value)
      {
         if (value) AutoFillConfinement();
         Recompute();
      }
      partial void OnLneoMmChanged(double value)          => Recompute();
      partial void OnLnoiMmChanged(double value)          => Recompute();

      // ─── Core recompute ──────────────────────────────────────────────────────

      private void Recompute()
      {
         try
         {
            var sec = _calculator.BuildSection(
               SectionB, SectionH, CoverMm, StirrupDiameter, LongBarDiameter,
               SelectedLayout, LongBarCount, BarsAlongB, BarsAlongH, _crossTieBars);

            var elev = _calculator.BuildElevation(
               sec, ColumnHeight,
               ConfinementLength, ConfinementSpacing, StirrupSpacing,
               LneoMm, LnoiMm);

            Section   = sec;
            Elevation = elev;

            var tieNote = _crossTieBars.Count > 0 ? $"  |  Đai phụ chữ C: {sec.CrossTies.Count}" : string.Empty;
            StatusMessage = (sec.Message ?? elev.Message ?? string.Empty) + tieNote;
         }
         catch (Exception ex)
         {
            _logger.Warning(ex, "Recompute failed");
            StatusMessage = $"Lỗi tính toán: {ex.Message}";
         }
      }

      private void AutoFillConfinement()
      {
         if (!AutoTcvn) return;
         var r = _calculator.ComputeConfinement(SectionB, SectionH, ColumnHeight, LongBarDiameter);
         ConfinementLength  = r.LengthMm;
         ConfinementSpacing = r.ConfSpacingMm;
         StirrupSpacing     = r.MidSpacingMm;
      }

      private void RebuildConfinementThenRecompute()
      {
         // Đổi đường kính đai → đổi inset → vị trí thanh dịch → đai phụ cũ không còn đúng.
         _crossTieBars.Clear();
         if (AutoTcvn) AutoFillConfinement();
         Recompute();
      }

      private void ClearCrossTiesAndRecompute()
      {
         _crossTieBars.Clear();
         Recompute();
      }

      /// <summary>
      ///     Toggle đai phụ chữ C cho 1 thanh thép dọc (gọi khi click thanh trên mặt cắt).
      ///     Đang có → bỏ; chưa có → thêm. Thanh góc sẽ bị calculator bỏ qua (đã có đai chính ôm).
      /// </summary>
      [RelayCommand]
      private void ToggleCrossTie(int barIndex)
      {
         if (barIndex < 0) return;
         if (!_crossTieBars.Remove(barIndex))
            _crossTieBars.Add(barIndex);
         Recompute();
      }

      /// <summary>
      ///     Ghi bộ thép hiện tại (thép dọc + đai kín + đai phụ chữ C) vào model Revit.
      ///     Chỉ khả dụng khi chạy trong Revit (CanCreateRebar). Lỗi được bắt và hiển thị ở StatusMessage.
      /// </summary>
      [RelayCommand(CanExecute = nameof(CanCreateRebar))]
      private void CreateRebar()
      {
         if (_createRebarAction == null) return;
         if (Section is not { IsValid: true } sec)
         {
            StatusMessage = "Mặt cắt chưa hợp lệ — chưa thể tạo thép.";
            return;
         }

         try
         {
            var zones = _calculator.ComputeStirrupZones(
               ColumnHeight, ConfinementLength, ConfinementSpacing, StirrupSpacing);

            var request = new RebarCreationRequest(
               SectionB, SectionH, ColumnHeight, CoverMm,
               LongBarDiameter, StirrupDiameter,
               sec.Bars, sec.CrossTies, zones, LneoMm);

            var r = _createRebarAction(request);

            if (r.Total == 0)
               StatusMessage = $"Không tạo được thép. Lỗi: {r.FirstError ?? "không rõ"}";
            else if (r.Failures > 0)
               StatusMessage = $"Đã tạo: {r.Longitudinal} dọc, {r.Stirrups} đai kín, {r.CrossTies} đai phụ. "
                             + $"{r.Failures} lỗi — {r.FirstError}";
            else
               StatusMessage = $"Đã tạo thép vào model: {r.Longitudinal} thanh dọc, "
                             + $"{r.Stirrups} đai kín, {r.CrossTies} đai phụ chữ C.";
         }
         catch (Exception ex)
         {
            _logger.Error(ex, "Tạo thép vào model thất bại");
            StatusMessage = $"Lỗi tạo thép: {ex.Message}";
         }
      }

      // ─── Commands ────────────────────────────────────────────────────────────

      [RelayCommand]
      private void SaveConfig()
      {
         var dto = new RebarConfigDto
         {
            SectionB          = SectionB,
            SectionH          = SectionH,
            ColumnHeightMm    = ColumnHeight,
            LongBarDiameter   = LongBarDiameter,
            LongBarCount      = LongBarCount,
            Layout            = SelectedLayout.ToString(),
            BarsAlongB        = BarsAlongB,
            BarsAlongH        = BarsAlongH,
            StirrupDiameter   = StirrupDiameter,
            StirrupSpacing    = StirrupSpacing,
            ConfinementSpacing = ConfinementSpacing,
            ConfinementLength  = ConfinementLength,
            AutoTcvn          = AutoTcvn,
            CoverMm           = CoverMm,
            Concrete          = SelectedConcrete.ToString(),
            Steel             = SelectedSteel.ToString(),
            LneoMm            = LneoMm,
            LnoiMm            = LnoiMm
         };
         _store.Save(dto);
      }

      [RelayCommand]
      private void LoadConfig()
      {
         var dto = _store.Load();
         if (dto == null) return;

         // Áp từng field — partial onChange sẽ trigger Recompute cuối cùng
         SectionB           = dto.SectionB;
         SectionH           = dto.SectionH;
         ColumnHeight       = dto.ColumnHeightMm;
         LongBarDiameter    = dto.LongBarDiameter > 0 ? dto.LongBarDiameter : LongBarDiameter;
         LongBarCount       = dto.LongBarCount > 0 ? dto.LongBarCount : LongBarCount;
         BarsAlongB         = dto.BarsAlongB > 0 ? dto.BarsAlongB : BarsAlongB;
         BarsAlongH         = dto.BarsAlongH > 0 ? dto.BarsAlongH : BarsAlongH;
         StirrupDiameter    = dto.StirrupDiameter > 0 ? dto.StirrupDiameter : StirrupDiameter;
         StirrupSpacing     = dto.StirrupSpacing > 0 ? dto.StirrupSpacing : StirrupSpacing;
         ConfinementSpacing = dto.ConfinementSpacing > 0 ? dto.ConfinementSpacing : ConfinementSpacing;
         ConfinementLength  = dto.ConfinementLength > 0 ? dto.ConfinementLength : ConfinementLength;
         AutoTcvn           = dto.AutoTcvn;
         CoverMm            = dto.CoverMm > 0 ? dto.CoverMm : CoverMm;
         LneoMm             = dto.LneoMm > 0 ? dto.LneoMm : LneoMm;
         LnoiMm             = dto.LnoiMm > 0 ? dto.LnoiMm : LnoiMm;

         if (Enum.TryParse<LongitudinalLayout>(dto.Layout, out var layout))
            SelectedLayout = layout;
         if (Enum.TryParse<ConcreteGrade>(dto.Concrete, out var concrete))
            SelectedConcrete = concrete;
         if (Enum.TryParse<SteelGroup>(dto.Steel, out var steel))
            SelectedSteel = steel;

         StatusMessage = "Đã tải cấu hình thép.";
      }
   }
}
