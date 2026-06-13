using Newtonsoft.Json.Linq;

namespace MyRevitAIApp.McpServer.Sdk;

public interface IRevitCommand
{
    string CommandName { get; }
    object Execute(JObject parameters, string requestId);
}
