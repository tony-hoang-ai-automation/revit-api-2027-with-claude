using Autodesk.Revit.UI;

namespace MyRevitAIApp.McpServer.Sdk;

public interface IWaitableExternalEventHandler : IExternalEventHandler
{
    bool WaitForCompletion(int timeoutMs);
}
