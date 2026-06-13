using Autodesk.Revit.UI;
using Newtonsoft.Json.Linq;

namespace MyRevitAIApp.McpServer.Sdk;

public abstract class ExternalEventCommandBase : IRevitCommand
{
    protected readonly IWaitableExternalEventHandler Handler;
    private readonly ExternalEvent _externalEvent;

    protected ExternalEventCommandBase(IWaitableExternalEventHandler handler, UIApplication uiApp)
    {
        Handler = handler;
        _externalEvent = ExternalEvent.Create(handler);
    }

    public abstract string CommandName { get; }
    public abstract object Execute(JObject parameters, string requestId);

    protected bool RaiseAndWaitForCompletion(int timeoutMs)
    {
        _externalEvent.Raise();
        return Handler.WaitForCompletion(timeoutMs);
    }
}
