using MyRevitAIApp.ViewModels;

namespace MyRevitAIApp.Views
{
   public sealed partial class MyRevitAIAppView
   {
      public MyRevitAIAppView(MyRevitAIAppViewModel viewModel)
      {
         DataContext = viewModel;
         InitializeComponent();
      }
   }
}