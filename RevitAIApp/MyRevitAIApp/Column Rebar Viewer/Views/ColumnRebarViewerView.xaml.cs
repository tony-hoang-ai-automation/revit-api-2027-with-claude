using MyRevitAIApp.ColumnRebarViewer.ViewModels;

namespace MyRevitAIApp.ColumnRebarViewer.Views
{
   public sealed partial class ColumnRebarViewerView
   {
      public ColumnRebarViewerView(ColumnRebarViewerViewModel viewModel)
      {
         DataContext = viewModel;
         InitializeComponent();
      }

      // Zoom/fit buttons delegate to both canvases simultaneously (view logic)
      private void BtnZoomIn_Click(object sender, System.Windows.RoutedEventArgs e)
      {
         SectionCanvas.ZoomIn();
         ElevationCanvas.ZoomIn();
      }

      private void BtnZoomOut_Click(object sender, System.Windows.RoutedEventArgs e)
      {
         SectionCanvas.ZoomOut();
         ElevationCanvas.ZoomOut();
      }

      private void BtnFit_Click(object sender, System.Windows.RoutedEventArgs e)
      {
         SectionCanvas.FitToView();
         ElevationCanvas.FitToView();
      }
   }
}
