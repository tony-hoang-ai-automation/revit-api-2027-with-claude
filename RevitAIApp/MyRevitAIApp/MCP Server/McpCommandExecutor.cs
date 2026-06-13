using Newtonsoft.Json.Linq;
using MyRevitAIApp.McpServer.Sdk;
using MyRevitAIApp.McpServer.Sdk.JsonRpc;

namespace MyRevitAIApp.McpServer;

public sealed class McpCommandExecutor
{
    private readonly ICommandRegistry _registry;

    public McpCommandExecutor(ICommandRegistry registry) => _registry = registry;

    public string Execute(JsonRPCRequest request)
    {
        if (!_registry.TryGetCommand(request.Method, out var command))
            return Error(request.Id, JsonRPCErrorCodes.MethodNotFound, $"Method '{request.Method}' not found");

        try
        {
            var result = command.Execute(request.GetParamsObject(), request.Id);
            return Success(request.Id, result);
        }
        catch (Exception ex)
        {
            return Error(request.Id, JsonRPCErrorCodes.InternalError, ex.Message);
        }
    }

    private static string Success(string id, object result) =>
        new JsonRPCSuccessResponse
        {
            Id = id,
            Result = result is JToken jt ? jt : JToken.FromObject(result)
        }.ToJson();

    private static string Error(string id, int code, string message) =>
        new JsonRPCErrorResponse
        {
            Id = id,
            Error = new JsonRPCError { Code = code, Message = message }
        }.ToJson();
}
