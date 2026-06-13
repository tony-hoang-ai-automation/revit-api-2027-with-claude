using System.Windows.Interop;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using MyRevitAIApp.DuplicateSheet.ViewModels;
using MyRevitAIApp.DuplicateSheet.Views;
using Nice3point.Revit.Toolkit.External;

namespace MyRevitAIApp.DuplicateSheet
{
   /// <summary>
   ///     Ribbon button entry point cho feature Duplicate Sheets.
   ///     Modal dialog, single Transaction wrap batch trong ViewModel.
   /// </summary>
   [UsedImplicitly]
   [Transaction(TransactionMode.Manual)]
   public class DuplicateSheetsCommand : ExternalCommand
   {
      public override void Execute()
      {
         var doc = Application.ActiveUIDocument?.Document;
         if (doc == null)
         {
            TaskDialog.Show("Duplicate Sheets", "Cần mở một document trước khi chạy command.");
            return;
         }

         var viewModel = new DuplicateSheetsViewModel(doc);
         var view = new DuplicateSheetsView(viewModel);
         new WindowInteropHelper(view) { Owner = Application.MainWindowHandle };
         view.ShowDialog();
      }
   }
}
