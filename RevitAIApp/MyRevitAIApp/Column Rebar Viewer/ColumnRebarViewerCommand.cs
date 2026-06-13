using System.Windows.Interop;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using MyRevitAIApp.ColumnRebarViewer.Services;
using MyRevitAIApp.ColumnRebarViewer.ViewModels;
using MyRevitAIApp.ColumnRebarViewer.Views;
using Nice3point.Revit.Toolkit.External;
using Serilog;

namespace MyRevitAIApp.ColumnRebarViewer
{
   /// <summary>
   ///     Ribbon button: người dùng pick 1 cột chữ nhật → mở dialog vẽ thép (preview).
   ///     Nút "Tạo thép vào model" ghi Rebar thật qua RebarCreationService (Transaction trong modal dialog).
   /// </summary>
   [UsedImplicitly]
   [Transaction(TransactionMode.Manual)]
   public class ColumnRebarViewerCommand : ExternalCommand
   {
      private static readonly ILogger Logger = Log.ForContext<ColumnRebarViewerCommand>();

      public override void Execute()
      {
         var uiDoc = Application.ActiveUIDocument;
         var doc   = uiDoc?.Document;
         if (doc == null)
         {
            TaskDialog.Show("Vẽ thép cột", "Cần mở một Revit document trước.");
            return;
         }

         // Pick 1 cột
         FamilyInstance? column;
         try
         {
            var filter = new StructuralColumnSelectionFilter();
            var ref_   = uiDoc!.Selection.PickObject(
               Autodesk.Revit.UI.Selection.ObjectType.Element, filter,
               "Chọn 1 cột kết cấu để vẽ thép — ESC để huỷ");
            column = doc.GetElement(ref_) as FamilyInstance;
         }
         catch (Autodesk.Revit.Exceptions.OperationCanceledException)
         {
            // Người dùng bấm ESC — không làm gì cả
            return;
         }

         if (column == null)
         {
            TaskDialog.Show("Vẽ thép cột", "Không đọc được phần tử đã chọn.");
            return;
         }

         // Đọc geometry
         var reader   = new ColumnGeometryReader();
         var geometry = reader.Read(column, doc.ActiveView);

         if (geometry.Shape == Models.RebarColumnShape.Round)
         {
            TaskDialog.Show("Vẽ thép cột",
               "Cột tròn chưa được hỗ trợ trong phiên bản này.\nVui lòng chọn cột chữ nhật.");
            return;
         }

         if (geometry.Shape == Models.RebarColumnShape.Unknown)
         {
            TaskDialog.Show("Vẽ thép cột",
               "Không nhận dạng được tiết diện cột.\nChỉ hỗ trợ cột chữ nhật.");
            return;
         }

         Logger.Information("Opening rebar viewer for column {Id} b={B:F0} h={H:F0} H={Ht:F0}mm",
            column.Id, geometry.BMm, geometry.HMm, geometry.HeightMm);

         var creator   = new RebarCreationService();
         var viewModel = new ColumnRebarViewerViewModel(
            geometry, request => creator.Create(column, request));
         var window    = new ColumnRebarViewerView(viewModel);
         new WindowInteropHelper(window) { Owner = Application.MainWindowHandle };
         window.ShowDialog();
      }
   }
}
