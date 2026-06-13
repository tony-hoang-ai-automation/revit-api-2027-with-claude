using System.Windows.Interop;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using MyRevitAIApp.GeometryToDirectShape.View;
using MyRevitAIApp.GeometryToDirectShape.ViewModels;
using Nice3point.Revit.Toolkit.External;
using RvtOperationCanceled = Autodesk.Revit.Exceptions.OperationCanceledException;

namespace MyRevitAIApp.GeometryToDirectShape
{
   /// <summary>
   ///     Ribbon button entry point cho feature "Geometry → DirectShape".
   ///     User pick 1 đối tượng → dialog cho chọn Instance/Symbol/cả hai → trích solid → vẽ DirectShape.
   /// </summary>
   [UsedImplicitly]
   [Transaction(TransactionMode.Manual)]
   public class GeometryToDirectShapeCommand : ExternalCommand
   {
      public override void Execute()
      {
         var uiDoc = Application.ActiveUIDocument;
         var doc = uiDoc?.Document;
         if (doc == null)
         {
            TaskDialog.Show("Geometry → DirectShape", "Cần mở một document trước khi chạy command.");
            return;
         }

         Reference reference;
         try
         {
            reference = uiDoc!.Selection.PickObject(ObjectType.Element,
               "Chọn 1 đối tượng để trích geometry solid và vẽ DirectShape");
         }
         catch (RvtOperationCanceled)
         {
            return; // user nhấn Esc — thoát êm
         }

         var element = doc.GetElement(reference);
         if (element == null)
         {
            TaskDialog.Show("Geometry → DirectShape", "Không lấy được phần tử đã chọn.");
            return;
         }

         var viewModel = new GeometryToDirectShapeViewModel(doc, element);
         var window = new GeometryToDirectShapeView(viewModel);
         new WindowInteropHelper(window) { Owner = Application.MainWindowHandle };
         window.ShowDialog();
      }
   }
}
