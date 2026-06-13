using Autodesk.Revit.Attributes;
using MyRevitAIApp.ViewModels;
using MyRevitAIApp.Views;
using Nice3point.Revit.Toolkit.External;

namespace MyRevitAIApp.Commands
{
   /// <summary>
   ///     External command entry point.
   /// </summary>
   [UsedImplicitly]
   [Transaction(TransactionMode.Manual)]
   public class StartupCommand : ExternalCommand
   {
      public override void Execute()
      {
         var viewModel = new MyRevitAIAppViewModel();
         var view = new MyRevitAIAppView(viewModel);
         view.ShowDialog();
      }
   }
}