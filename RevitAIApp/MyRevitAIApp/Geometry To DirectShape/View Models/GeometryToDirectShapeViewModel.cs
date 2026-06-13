using System.Collections.ObjectModel;
using Autodesk.Revit.UI;
using MyRevitAIApp.GeometryToDirectShape.Models;
using Serilog;

namespace MyRevitAIApp.GeometryToDirectShape.ViewModels
{
   /// <summary>
   ///     ViewModel cho dialog "Geometry → DirectShape". Chạy trong modal dialog (gọi từ command)
   ///     nên transaction thực hiện trực tiếp trên thread Revit API. Cho phép tạo DirectShape theo
   ///     Instance / Symbol / cả hai để so sánh khác biệt vị trí.
   /// </summary>
   public sealed partial class GeometryToDirectShapeViewModel : ObservableObject
   {
      private readonly Document _doc;
      private readonly Element _source;
      private readonly DirectShapeBuilderService _builder = new();
      private readonly ILogger _logger = Log.ForContext<GeometryToDirectShapeViewModel>();

      public GeometryToDirectShapeViewModel(Document doc, Element source)
      {
         _doc = doc;
         _source = source;
         SourceDescription = BuildSourceDescription(source);
         Results = new ObservableCollection<DirectShapeCreationResult>();
      }

      /// <summary>Mô tả phần tử đã chọn (category, tên, loại) để hiển thị đầu dialog.</summary>
      public string SourceDescription { get; }

      /// <summary>Bảng kết quả các DirectShape đã tạo — cột vị trí tâm cho thấy khác biệt Instance/Symbol.</summary>
      public ObservableCollection<DirectShapeCreationResult> Results { get; }

      // Radio mode — RadioButton cùng GroupName trong XAML tự đảm bảo loại trừ lẫn nhau.
      [ObservableProperty] private bool _isInstanceMode = true;
      [ObservableProperty] private bool _isSymbolMode;
      [ObservableProperty] private bool _isBothMode;

      [ObservableProperty] private string _statusMessage = "Chọn nguồn geometry rồi bấm \"Tạo DirectShape\".";

      [RelayCommand]
      private void Create()
      {
         try
         {
            if (IsBothMode)
            {
               var instance = _builder.Create(_doc, _source, GeometrySourceMode.Instance);
               var symbol = _builder.Create(_doc, _source, GeometrySourceMode.Symbol);
               Results.Add(instance);
               Results.Add(symbol);
               StatusMessage = instance.SolidCount > 0
                  ? "Đã tạo 2 DirectShape (Instance + Symbol). So sánh cột \"Tâm (mm)\" ở bảng dưới."
                  : "Không trích được solid nào từ phần tử đã chọn.";
            }
            else
            {
               var mode = IsSymbolMode ? GeometrySourceMode.Symbol : GeometrySourceMode.Instance;
               var result = _builder.Create(_doc, _source, mode);
               Results.Add(result);
               StatusMessage = result.SolidCount > 0
                  ? $"Đã tạo DirectShape {result.ModeLabel} với {result.SolidCount} solid (Id {result.DirectShapeId})."
                  : "Không trích được solid nào từ phần tử đã chọn.";
            }
         }
         catch (Exception ex)
         {
            _logger.Error(ex, "Tạo DirectShape thất bại");
            StatusMessage = $"Lỗi: {ex.Message}";
            TaskDialog.Show("Geometry → DirectShape", ex.Message);
         }
      }

      private static string BuildSourceDescription(Element source)
      {
         var category = source.Category?.Name ?? "(không có category)";
         var type = source is FamilyInstance fi
            ? $"FamilyInstance · {fi.Symbol?.FamilyName}"
            : source.GetType().Name;
         return $"{category} — {source.Name}  [{type}]";
      }
   }
}
