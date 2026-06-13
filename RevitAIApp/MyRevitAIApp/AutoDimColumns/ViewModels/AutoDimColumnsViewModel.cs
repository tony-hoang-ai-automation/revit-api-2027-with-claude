using System.Collections.ObjectModel;
using Autodesk.Revit.UI;
using MyRevitAIApp.AutoDimColumns.Internals;
using MyRevitAIApp.AutoDimColumns.Models;
using MyRevitAIApp.AutoDimColumns.Services;
using Serilog;

namespace MyRevitAIApp.AutoDimColumns.ViewModels
{
   /// <summary>
   ///     ViewModel cho dialog AutoDimColumns. Owns service instances (manual new),
   ///     orchestrates Preview + Run flows. Transaction wrap toàn batch trong <see cref="ExecuteRun"/>.
   /// </summary>
   public sealed partial class AutoDimColumnsViewModel : ObservableObject
   {
      private readonly Document _doc;
      private readonly View _activeView;
      private readonly IList<ElementId>? _selectedIds;

      private readonly IColumnCollector _columnCollector = new ColumnCollector();
      private readonly IGridCollector _gridCollector = new GridCollector();
      private readonly IDimensionPlanner _planner = new DimensionPlanner();
      private readonly IColumnReferenceExtractor _refExtractor = new ColumnReferenceExtractor();
      private readonly IExistingDimensionCleaner _cleaner = new ExistingDimensionCleaner();
      private readonly IDimensionExecutor _executor;
      private readonly ISettingsStore _settingsStore = new SettingsStore();
      private readonly ILogger _logger = Log.ForContext<AutoDimColumnsViewModel>();

      private IReadOnlyDictionary<long, FamilyInstance> _columnIndex = new Dictionary<long, FamilyInstance>();
      private IReadOnlyDictionary<long, Grid> _gridIndex = new Dictionary<long, Grid>();
      private IReadOnlyList<DimSpec> _currentSpecs = Array.Empty<DimSpec>();

      public AutoDimColumnsViewModel(Document doc, View activeView, IList<ElementId>? selectedIds)
      {
         _doc = doc;
         _activeView = activeView;
         _selectedIds = selectedIds;
         _executor = new DimensionExecutor(_refExtractor, _cleaner);

         AvailableLinearDimTypes = new ObservableCollection<DimensionType>(LoadLinearDimTypes());
         AvailableRadialDimTypes = new ObservableCollection<DimensionType>(LoadRadialDimTypes());
         AvailableScopes = (ScopeMode[])Enum.GetValues(typeof(ScopeMode));
         AvailableExistingPolicies = (ExistingDimsPolicy[])Enum.GetValues(typeof(ExistingDimsPolicy));
         PreviewItems = new ObservableCollection<DimPreviewItem>();

         LoadSettings(_settingsStore.Load());
      }

      // ============ Reference data ============

      public ObservableCollection<DimensionType> AvailableLinearDimTypes { get; }
      public ObservableCollection<DimensionType> AvailableRadialDimTypes { get; }
      public ScopeMode[] AvailableScopes { get; }
      public ExistingDimsPolicy[] AvailableExistingPolicies { get; }

      // ============ Settings bindings ============

      [ObservableProperty] private DimensionType? _selectedLinearDimType;
      [ObservableProperty] private DimensionType? _selectedRadialDimType;
      [ObservableProperty] private double _faceOffsetMm = 500.0;
      [ObservableProperty] private ScopeMode _selectedScope = ScopeMode.ActiveView;
      [ObservableProperty] private bool _dimAxisX = true;
      [ObservableProperty] private bool _dimAxisY = true;
      [ObservableProperty] private bool _includeSelfDim = true;
      [ObservableProperty] private bool _includeRadialDim = true;
      [ObservableProperty] private bool _includePerimeterDim = true;
      [ObservableProperty] private ExistingDimsPolicy _selectedExistingPolicy = ExistingDimsPolicy.DeleteThenRecreate;

      partial void OnSelectedLinearDimTypeChanged(DimensionType? value) => MarkPreviewStale();
      partial void OnSelectedRadialDimTypeChanged(DimensionType? value) => MarkPreviewStale();
      partial void OnFaceOffsetMmChanged(double value) => MarkPreviewStale();
      partial void OnSelectedScopeChanged(ScopeMode value) => MarkPreviewStale();
      partial void OnDimAxisXChanged(bool value) => MarkPreviewStale();
      partial void OnDimAxisYChanged(bool value) => MarkPreviewStale();
      partial void OnIncludeSelfDimChanged(bool value) => MarkPreviewStale();
      partial void OnIncludeRadialDimChanged(bool value) => MarkPreviewStale();
      partial void OnIncludePerimeterDimChanged(bool value) => MarkPreviewStale();

      // ============ Preview + state ============

      public ObservableCollection<DimPreviewItem> PreviewItems { get; }

      [ObservableProperty]
      [NotifyCanExecuteChangedFor(nameof(RunCommand))]
      private bool _isPreviewStale = true;

      [ObservableProperty]
      [NotifyCanExecuteChangedFor(nameof(RunCommand))]
      [NotifyCanExecuteChangedFor(nameof(PreviewCommand))]
      private bool _isBusy;

      [ObservableProperty] private string _statusMessage = "Sẵn sàng. Bấm Preview để xem trước.";
      [ObservableProperty] private int _existingDimCount;

      // ============ Commands ============

      [RelayCommand(CanExecute = nameof(CanPreview))]
      private void Preview()
      {
         try
         {
            IsBusy = true;
            PreviewItems.Clear();
            var columns = _columnCollector.Collect(_doc, _activeView, SelectedScope, _selectedIds);
            var grids = _gridCollector.Collect(_doc, _activeView);
            _columnIndex = columns.InstanceLookup;
            _gridIndex = grids.GridLookup;

            if (columns.Snapshots.Count == 0)
            {
               StatusMessage = "Không tìm thấy cột trong scope đã chọn.";
               IsPreviewStale = false;
               return;
            }

            var settings = BuildSettings();
            _currentSpecs = _planner.Plan(columns.Snapshots, grids.Snapshots, settings);

            ExistingDimCount = settings.ExistingDims == ExistingDimsPolicy.DeleteThenRecreate
               ? _cleaner.CountOverlapping(_doc, _activeView, _currentSpecs.Where(s => s.ColumnId.HasValue).Select(s => s.ColumnId!.Value).Distinct())
               : 0;

            var idx = 1;
            foreach (var s in _currentSpecs)
            {
               PreviewItems.Add(new DimPreviewItem(
                  Index: idx++,
                  Column: s.ColumnLabel,
                  Kind: FormatKind(s.Kind),
                  Target: s.GridsLabel,
                  DimStyle: s.Kind == DimRequestKind.RadialRound
                     ? SelectedRadialDimType?.Name ?? "—"
                     : SelectedLinearDimType?.Name ?? "—",
                  IsSkipped: s.SkipReason != null,
                  Note: s.SkipReason ?? "OK"));
            }

            var skipped = _currentSpecs.Count(s => s.SkipReason != null);
            StatusMessage = $"Preview: {_currentSpecs.Count - skipped} dim sẽ tạo, {skipped} bị skip. Existing dim overlap: {ExistingDimCount}.";
            IsPreviewStale = false;
         }
         catch (Exception ex)
         {
            _logger.Error(ex, "Preview failed");
            StatusMessage = $"Lỗi preview: {ex.Message}";
         }
         finally
         {
            IsBusy = false;
         }
      }

      [RelayCommand(CanExecute = nameof(CanRun))]
      private void Run()
      {
         if (SelectedLinearDimType == null)
         {
            TaskDialog.Show("AutoDim Columns", "Vui lòng chọn Linear Dim Type trước khi chạy.");
            return;
         }

         if (SelectedExistingPolicy == ExistingDimsPolicy.DeleteThenRecreate && ExistingDimCount > 0)
         {
            var result = TaskDialog.Show(
               "AutoDim Columns — Xác nhận xoá",
               $"Sẽ xoá {ExistingDimCount} dim cũ trước khi tạo lại. Tiếp tục?",
               TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No);
            if (result != TaskDialogResult.Yes) return;
         }

         try
         {
            IsBusy = true;
            using var t = new Transaction(_doc, "Auto-Dim Columns");
            t.Start();
            var settings = BuildSettings();
            var radialType = SelectedRadialDimType ?? SelectedLinearDimType!;
            var outcome = _executor.Execute(_doc, _activeView, _currentSpecs, settings, _columnIndex, _gridIndex, SelectedLinearDimType!, radialType);
            t.Commit();

            _settingsStore.Save(settings);

            var msg = $"Tạo: {outcome.Created} dim • Skip: {outcome.Skipped} • Fail: {outcome.Failed} • Xoá dim cũ: {outcome.ExistingDimsDeleted}";
            StatusMessage = msg;
            TaskDialog.Show("AutoDim Columns — Hoàn tất", msg + (outcome.Errors.Count > 0 ? $"\n\nLỗi đầu tiên:\n{outcome.Errors[0]}" : string.Empty));
            IsPreviewStale = true;
         }
         catch (Exception ex)
         {
            _logger.Error(ex, "Run failed");
            TaskDialog.Show("AutoDim Columns — Lỗi", ex.Message);
            StatusMessage = $"Lỗi: {ex.Message}";
         }
         finally
         {
            IsBusy = false;
         }
      }

      // ============ Internals ============

      private DimensioningSettings BuildSettings() => new(
         LinearDimTypeId: SelectedLinearDimType?.Id.GetIdValue(),
         RadialDimTypeId: SelectedRadialDimType?.Id.GetIdValue(),
         FaceOffsetMm: FaceOffsetMm,
         Scope: SelectedScope,
         DimAxisX: DimAxisX,
         DimAxisY: DimAxisY,
         IncludeSelfDim: IncludeSelfDim,
         IncludeRadialDim: IncludeRadialDim,
         IncludePerimeterDim: IncludePerimeterDim,
         ExistingDims: SelectedExistingPolicy,
         RotationToleranceDegrees: 1.0,
         OnGridToleranceMm: 5.0);

      private void LoadSettings(DimensioningSettings s)
      {
         FaceOffsetMm = s.FaceOffsetMm;
         SelectedScope = s.Scope;
         DimAxisX = s.DimAxisX;
         DimAxisY = s.DimAxisY;
         IncludeSelfDim = s.IncludeSelfDim;
         IncludeRadialDim = s.IncludeRadialDim;
         IncludePerimeterDim = s.IncludePerimeterDim;
         SelectedExistingPolicy = s.ExistingDims;

         if (s.LinearDimTypeId.HasValue)
         {
            SelectedLinearDimType = AvailableLinearDimTypes.FirstOrDefault(d => d.Id.GetIdValue() == s.LinearDimTypeId.Value)
                                    ?? AvailableLinearDimTypes.FirstOrDefault();
         }
         else
         {
            SelectedLinearDimType = AvailableLinearDimTypes.FirstOrDefault();
         }

         if (s.RadialDimTypeId.HasValue)
         {
            SelectedRadialDimType = AvailableRadialDimTypes.FirstOrDefault(d => d.Id.GetIdValue() == s.RadialDimTypeId.Value)
                                    ?? AvailableRadialDimTypes.FirstOrDefault();
         }
         else
         {
            SelectedRadialDimType = AvailableRadialDimTypes.FirstOrDefault();
         }
      }

      private IEnumerable<DimensionType> LoadLinearDimTypes() =>
         new FilteredElementCollector(_doc)
            .OfClass(typeof(DimensionType))
            .Cast<DimensionType>()
            .Where(d => d.StyleType == DimensionStyleType.Linear || d.StyleType == DimensionStyleType.LinearFixed)
            .OrderBy(d => d.Name, StringComparer.OrdinalIgnoreCase);

      private IEnumerable<DimensionType> LoadRadialDimTypes() =>
         new FilteredElementCollector(_doc)
            .OfClass(typeof(DimensionType))
            .Cast<DimensionType>()
            .Where(d => d.StyleType == DimensionStyleType.Radial)
            .OrderBy(d => d.Name, StringComparer.OrdinalIgnoreCase);

      private void MarkPreviewStale() => IsPreviewStale = true;

      private bool CanPreview() => !IsBusy;

      private bool CanRun() => !IsBusy && !IsPreviewStale && PreviewItems.Count > 0;

      private static string FormatKind(DimRequestKind k) => k switch
      {
         DimRequestKind.SelfDimSquareX => "Self-X",
         DimRequestKind.SelfDimSquareY => "Self-Y",
         DimRequestKind.ColumnToGridX => "To-Grid-X",
         DimRequestKind.ColumnToGridY => "To-Grid-Y",
         DimRequestKind.RadialRound => "Radial",
         DimRequestKind.PerimeterTotalX => "Perimeter-X",
         DimRequestKind.PerimeterTotalY => "Perimeter-Y",
         _ => k.ToString()
      };
   }
}
