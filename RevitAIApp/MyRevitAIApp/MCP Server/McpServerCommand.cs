using Autodesk.Revit.UI;
using Nice3point.Revit.Toolkit.External;

namespace MyRevitAIApp.McpServer;

[UsedImplicitly]
[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class McpServerCommand : ExternalCommand
{
    public override void Execute()
    {
        var service = McpSocketService.Instance;

        if (service.IsRunning)
        {
            service.Stop();
            TaskDialog.Show("MCP Server", "MCP Server đã tắt.");
        }
        else
        {
            service.Initialize(UiApplication);
            service.Start();
            TaskDialog.Show("MCP Server", "MCP Server đã bật — port 8080.");
        }
    }
}
