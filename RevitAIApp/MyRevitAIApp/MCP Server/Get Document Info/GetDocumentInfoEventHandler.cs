using System;
using System.Threading;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MyRevitAIApp.McpServer.Sdk;

namespace MyRevitAIApp.McpServer.GetDocumentInfo
{
    /// <summary>
    /// Read-only handler returning high-level information about the active
    /// Revit document (title, path, sheet/view counts). No transaction needed.
    /// Used as the smoke-test command for the project-specific command set path.
    /// </summary>
    public class GetDocumentInfoEventHandler : IExternalEventHandler, IWaitableExternalEventHandler
    {
        private readonly ManualResetEvent _resetEvent = new ManualResetEvent(false);

        public object Result { get; private set; }

        public bool WaitForCompletion(int timeoutMilliseconds = 10000)
        {
            _resetEvent.Reset();
            return _resetEvent.WaitOne(timeoutMilliseconds);
        }

        public void Execute(UIApplication app)
        {
            try
            {
                Document doc = app.ActiveUIDocument?.Document;
                if (doc == null)
                {
                    Result = new { success = false, message = "No active document. Open a Revit model first." };
                    return;
                }

                int sheetCount = new FilteredElementCollector(doc)
                    .OfClass(typeof(ViewSheet))
                    .WhereElementIsNotElementType()
                    .GetElementCount();

                int viewCount = new FilteredElementCollector(doc)
                    .OfClass(typeof(View))
                    .WhereElementIsNotElementType()
                    .GetElementCount();

                Result = new
                {
                    success = true,
                    title = doc.Title,
                    pathName = doc.PathName,
                    isWorkshared = doc.IsWorkshared,
                    isFamilyDocument = doc.IsFamilyDocument,
                    activeViewName = doc.ActiveView?.Name,
                    sheetCount,
                    viewCount
                };
            }
            catch (Exception ex)
            {
                Result = new { success = false, message = ex.Message };
            }
            finally
            {
                _resetEvent.Set();
            }
        }

        public string GetName() => "Get Document Info";
    }
}
