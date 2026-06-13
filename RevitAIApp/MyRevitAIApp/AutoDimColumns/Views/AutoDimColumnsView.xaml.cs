using MyRevitAIApp.AutoDimColumns.ViewModels;

namespace MyRevitAIApp.AutoDimColumns.Views
{
   public sealed partial class AutoDimColumnsView
   {
      public AutoDimColumnsView(AutoDimColumnsViewModel viewModel)
      {
         DataContext = viewModel;
         InitializeComponent();
      }
   }
}
