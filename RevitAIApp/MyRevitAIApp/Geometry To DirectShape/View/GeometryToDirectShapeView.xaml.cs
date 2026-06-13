using MyRevitAIApp.GeometryToDirectShape.ViewModels;

namespace MyRevitAIApp.GeometryToDirectShape.View
{
   public sealed partial class GeometryToDirectShapeView
   {
      public GeometryToDirectShapeView(GeometryToDirectShapeViewModel viewModel)
      {
         DataContext = viewModel;
         InitializeComponent();
      }
   }
}
