using MyRevitAIApp.DuplicateSheet.ViewModels;

namespace MyRevitAIApp.DuplicateSheet.Views
{
   public sealed partial class DuplicateSheetsView
   {
      public DuplicateSheetsView(DuplicateSheetsViewModel viewModel)
      {
         DataContext = viewModel;
         InitializeComponent();
      }
   }
}
