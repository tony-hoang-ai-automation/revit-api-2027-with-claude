using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Nice3point.Revit.Toolkit.External;

namespace MyRevitAIApp.Commands
{
   /// <summary>
   ///     Displays information about the active Revit document and host application.
   /// </summary>
   [UsedImplicitly]
   [Transaction(TransactionMode.Manual)]
   public class DocumentInfoCommand : ExternalCommand
   {
      public override void Execute()
      {
         var uiApp = Application;
         var uiDoc = uiApp.ActiveUIDocument;
         if (uiDoc is null)
         {
            TaskDialog.Show("Document Info", "Không có document nào đang mở.");
            return;
         }

         var doc = uiDoc.Document;
         var revitApp = uiApp.Application;

         var path = string.IsNullOrEmpty(doc.PathName) ? "(unsaved)" : doc.PathName;
         var viewName = uiDoc.ActiveView?.Name ?? "(none)";

         var message =
             $"Title: {doc.Title}\n" +
             $"Path: {path}\n" +
             $"Active View: {viewName}\n" +
             $"Revit: {revitApp.VersionName} (build {revitApp.VersionBuild})";

         TaskDialog.Show("Document Info", message);
      }
   }
}
