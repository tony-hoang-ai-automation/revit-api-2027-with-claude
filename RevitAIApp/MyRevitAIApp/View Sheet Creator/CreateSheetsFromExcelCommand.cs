using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Nice3point.Revit.Toolkit.External;

namespace MyRevitAIApp.ViewSheetCreator
{
   /// <summary>
   ///     Ribbon button entry point cho feature "View Sheet Creator".
   ///     Import Excel (4 cột: SheetNumber, SheetName, LoaiToa, TitleBlock) →
   ///     preview dialog → batch tạo ViewSheets với TransactionGroup.Assimilate.
   /// </summary>
   [UsedImplicitly]
   [Transaction(TransactionMode.Manual)]
   public class CreateSheetsFromExcelCommand : ExternalCommand
   {
      public override void Execute()
      {
         var doc = Application.ActiveUIDocument?.Document;
         if (doc == null)
         {
            TaskDialog.Show("View Sheet Creator", "Cần mở một document trước khi chạy command.");
            return;
         }

         TaskDialog.Show("View Sheet Creator", "Phase 1 skeleton — sẽ implement Excel import + preview dialog ở các phase tiếp.");
      }
   }
}
