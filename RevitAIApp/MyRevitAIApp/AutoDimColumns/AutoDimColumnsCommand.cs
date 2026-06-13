using System.Windows.Interop;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using MyRevitAIApp.AutoDimColumns.ViewModels;
using MyRevitAIApp.AutoDimColumns.Views;
using Nice3point.Revit.Toolkit.External;

namespace MyRevitAIApp.AutoDimColumns
{
   /// <summary>
   ///     Ribbon button entry point cho feature Auto-Dim Columns.
   ///     Validate active view (FloorPlan, !IsTemplate) trước khi mở dialog.
   /// </summary>
   [UsedImplicitly]
   [Transaction(TransactionMode.Manual)]
   public class AutoDimColumnsCommand : ExternalCommand
   {
      public override void Execute()
      {
         var uiDoc = Application.ActiveUIDocument;
         var doc = uiDoc?.Document;
         if (doc == null)
         {
            TaskDialog.Show("Auto-Dim Columns", "Cần mở một document trước khi chạy command.");
            return;
         }

         var view = doc.ActiveView;
         if (view.ViewType != ViewType.FloorPlan || view.IsTemplate)
         {
            TaskDialog.Show("Auto-Dim Columns",
               "Vui lòng kích hoạt 1 Floor Plan view (không phải template) trước khi chạy command.");
            return;
         }

         var selectedIds = uiDoc!.Selection.GetElementIds().ToList();

         var viewModel = new AutoDimColumnsViewModel(doc, view, selectedIds);
         var window = new AutoDimColumnsView(viewModel);
         new WindowInteropHelper(window) { Owner = Application.MainWindowHandle };
         window.ShowDialog();
      }
   }
}
