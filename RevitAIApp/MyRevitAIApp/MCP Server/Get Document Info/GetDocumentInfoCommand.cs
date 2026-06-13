using System;
using Autodesk.Revit.UI;
using Newtonsoft.Json.Linq;
using MyRevitAIApp.McpServer.Sdk;
using MyRevitAIApp.McpServer.GetDocumentInfo;

namespace MyRevitAIApp.McpServer.GetDocumentInfo
{
    /// <summary>
    /// Project-specific smoke-test command. Returns high-level info about the
    /// active document so the TS tool ⇄ C# command path can be validated
    /// end-to-end before wrapping the real feature services.
    /// </summary>
    public class GetDocumentInfoCommand : ExternalEventCommandBase
    {
        private GetDocumentInfoEventHandler _handler => (GetDocumentInfoEventHandler)Handler;

        public override string CommandName => "get_document_info";

        public GetDocumentInfoCommand(UIApplication uiApp)
            : base(new GetDocumentInfoEventHandler(), uiApp)
        {
        }

        public override object Execute(JObject parameters, string requestId)
        {
            try
            {
                if (RaiseAndWaitForCompletion(10000))
                {
                    return _handler.Result;
                }

                throw new TimeoutException("Get document info timed out");
            }
            catch (Exception ex)
            {
                throw new Exception($"Get document info failed: {ex.Message}");
            }
        }
    }
}
