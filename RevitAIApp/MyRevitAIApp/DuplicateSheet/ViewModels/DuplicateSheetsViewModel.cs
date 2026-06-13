using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using Autodesk.Revit.UI;
using MyRevitAIApp.DuplicateSheet.Models;
using MyRevitAIApp.DuplicateSheet.Services;
using Serilog;

namespace MyRevitAIApp.DuplicateSheet.ViewModels
{
   /// <summary>
   ///     ViewModel cho dialog Duplicate Sheets. Coordinator giữa UI ↔ NamingRuleEngine + SheetDuplicator.
   ///     Transaction wrap toàn batch trong <see cref="Execute"/>, fail = rollback all.
   /// </summary>
   public sealed partial class DuplicateSheetsViewModel : ObservableObject
   {
      private readonly Document _doc;
      private readonly INamingRuleEngine _namingEngine = new NamingRuleEngine();
      private readonly ISheetDuplicator _duplicator = new SheetDuplicator();
      private readonly ILogger _logger = Log.ForContext<DuplicateSheetsViewModel>();

      public DuplicateSheetsViewModel(Document doc)
      {
         _doc = doc;
         Sheets = new ObservableCollection<SheetItem>(LoadSheetsFromDocument());
         SheetsView = CollectionViewSource.GetDefaultView(Sheets);
         SheetsView.Filter = MatchesSearchText;
         AvailableModes = (DuplicateMode[])Enum.GetValues(typeof(DuplicateMode));
         PreviewItems = new ObservableCollection<PreviewItem>();
      }

      // ============ Sheet list ============

      public ObservableCollection<SheetItem> Sheets { get; }

      public ICollectionView SheetsView { get; }

      [ObservableProperty]
      private string _searchText = string.Empty;

      partial void OnSearchTextChanged(string value) => SheetsView.Refresh();

      // ============ Naming rule (10 fields) ============

      [ObservableProperty] private string _numberPrefix = string.Empty;
      [ObservableProperty] private string _numberSuffix = string.Empty;
      [ObservableProperty] private string _numberFind = string.Empty;
      [ObservableProperty] private string _numberReplace = string.Empty;
      [ObservableProperty] private int _numberIncrementStart;
      [ObservableProperty] private int _numberIncrementPad;
      [ObservableProperty] private string _namePrefix = string.Empty;
      [ObservableProperty] private string _nameSuffix = string.Empty;
      [ObservableProperty] private string _nameFind = string.Empty;
      [ObservableProperty] private string _nameReplace = string.Empty;

      partial void OnNumberPrefixChanged(string value) => MarkPreviewStale();
      partial void OnNumberSuffixChanged(string value) => MarkPreviewStale();
      partial void OnNumberFindChanged(string value) => MarkPreviewStale();
      partial void OnNumberReplaceChanged(string value) => MarkPreviewStale();
      partial void OnNumberIncrementStartChanged(int value) => MarkPreviewStale();
      partial void OnNumberIncrementPadChanged(int value) => MarkPreviewStale();
      partial void OnNamePrefixChanged(string value) => MarkPreviewStale();
      partial void OnNameSuffixChanged(string value) => MarkPreviewStale();
      partial void OnNameFindChanged(string value) => MarkPreviewStale();
      partial void OnNameReplaceChanged(string value) => MarkPreviewStale();

      // ============ Mode + content options ============

      public DuplicateMode[] AvailableModes { get; }

      [ObservableProperty] private DuplicateMode _selectedMode = DuplicateMode.WithDetailing;
      [ObservableProperty] private bool _copyTitleBlock = true;
      [ObservableProperty] private bool _includeLegends = true;
      [ObservableProperty] private bool _includeSchedules = true;
      [ObservableProperty] private bool _copySheetAnnotations = true;

      // ============ Preview + state ============

      public ObservableCollection<PreviewItem> PreviewItems { get; }

      [ObservableProperty]
      [NotifyCanExecuteChangedFor(nameof(ExecuteCommand))]
      private bool _isPreviewStale = true;

      [ObservableProperty]
      [NotifyCanExecuteChangedFor(nameof(ExecuteCommand))]
      [NotifyCanExecuteChangedFor(nameof(RefreshPreviewCommand))]
      private bool _isBusy;

      [ObservableProperty]
      private string _statusMessage = string.Empty;

      // ============ Commands ============

      [RelayCommand(CanExecute = nameof(CanRefreshPreview))]
      private void RefreshPreview()
      {
         PreviewItems.Clear();
         var selected = Sheets.Where(s => s.IsSelected).ToList();
         if (selected.Count == 0)
         {
            StatusMessage = "Chọn ít nhất 1 sheet để preview.";
            IsPreviewStale = false;
            return;
         }

         var rule = BuildNamingRule();
         var taken = new HashSet<string>(GetExistingSheetNumbers(), StringComparer.OrdinalIgnoreCase);

         for (var i = 0; i < selected.Count; i++)
         {
            var src = selected[i];
            var (newNumber, newName, collided) = _namingEngine.Apply(src.Number, src.Name, i, rule, taken);
            taken.Add(newNumber);

            PreviewItems.Add(new PreviewItem(
               src.Number, src.Name, newNumber, newName,
               collided,
               IsBlockingError: false,
               StatusMessage: collided ? "Trùng số → auto +(n)" : "OK"));
         }

         StatusMessage = $"Preview: {selected.Count} sheet sẽ được duplicate.";
         IsPreviewStale = false;
      }

      [RelayCommand(CanExecute = nameof(CanExecute))]
      private void Execute()
      {
         IsBusy = true;
         try
         {
            ExecuteBatch();
         }
         catch (Exception ex)
         {
            _logger.Error(ex, "Batch execute failed");
            TaskDialog.Show("Duplicate Sheets — Lỗi", ex.Message);
         }
         finally
         {
            IsBusy = false;
         }
      }

      // ============ Internals ============

      private void ExecuteBatch()
      {
         var selected = Sheets.Where(s => s.IsSelected).ToList();
         var content = new SheetContentOptions(CopyTitleBlock, IncludeLegends, IncludeSchedules, CopySheetAnnotations);

         using var t = new Transaction(_doc, "Duplicate Sheets");
         t.Start();
         var outcomes = new List<DuplicationOutcome>();
         try
         {
            for (var i = 0; i < selected.Count; i++)
            {
               var preview = PreviewItems[i];
               var src = _doc.GetElement(selected[i].Id) as ViewSheet;
               if (src == null) throw new InvalidOperationException($"Source sheet {preview.SourceNumber} không tồn tại.");

               var outcome = _duplicator.DuplicateOne(_doc, src, preview.NewNumber, preview.NewName, SelectedMode, content);
               outcomes.Add(outcome);
               if (!outcome.Success)
               {
                  throw new InvalidOperationException($"Sheet {preview.SourceNumber}: {outcome.ErrorMessage}");
               }
            }
            t.Commit();
            StatusMessage = $"Hoàn tất: {outcomes.Count} sheet đã tạo.";
            TaskDialog.Show("Duplicate Sheets", $"Đã duplicate {outcomes.Count} sheet thành công.");
         }
         catch
         {
            t.RollBack();
            throw;
         }
      }

      private NamingRule BuildNamingRule() => new(
         NullIfEmpty(NumberPrefix), NullIfEmpty(NumberSuffix),
         NullIfEmpty(NumberFind), NullIfEmpty(NumberReplace),
         NumberIncrementStart, NumberIncrementPad,
         NullIfEmpty(NamePrefix), NullIfEmpty(NameSuffix),
         NullIfEmpty(NameFind), NullIfEmpty(NameReplace));

      private static string? NullIfEmpty(string? s) => string.IsNullOrEmpty(s) ? null : s;

      private void MarkPreviewStale() => IsPreviewStale = true;

      private IEnumerable<string> GetExistingSheetNumbers() =>
         new FilteredElementCollector(_doc)
            .OfClass(typeof(ViewSheet))
            .Cast<ViewSheet>()
            .Where(s => !s.IsTemplate && !s.IsPlaceholder)
            .Select(s => s.SheetNumber);

      private IEnumerable<SheetItem> LoadSheetsFromDocument() =>
         new FilteredElementCollector(_doc)
            .OfClass(typeof(ViewSheet))
            .Cast<ViewSheet>()
            .Where(s => !s.IsTemplate && !s.IsPlaceholder)
            .OrderBy(s => s.SheetNumber, StringComparer.OrdinalIgnoreCase)
            .Select(s => CreateSheetItem(s));

      private SheetItem CreateSheetItem(ViewSheet s)
      {
         var item = new SheetItem(s.Id, s.SheetNumber, s.Name);
         item.PropertyChanged += OnSheetItemSelectionChanged;
         return item;
      }

      private void OnSheetItemSelectionChanged(object? sender, PropertyChangedEventArgs e)
      {
         if (e.PropertyName == nameof(SheetItem.IsSelected))
         {
            MarkPreviewStale();
         }
      }

      private bool MatchesSearchText(object obj)
      {
         if (string.IsNullOrWhiteSpace(SearchText)) return true;
         if (obj is not SheetItem item) return false;
         var q = SearchText.Trim();
         return item.Number.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0
                || item.Name.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0;
      }

      private bool CanRefreshPreview() => !IsBusy;

      private bool CanExecute() =>
         !IsBusy
         && !IsPreviewStale
         && PreviewItems.Count > 0
         && PreviewItems.All(p => !p.IsBlockingError);
   }
}
