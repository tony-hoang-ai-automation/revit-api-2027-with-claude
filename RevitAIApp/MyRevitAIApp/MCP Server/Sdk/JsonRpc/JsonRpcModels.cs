using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MyRevitAIApp.McpServer.Sdk.JsonRpc;

public class JsonRPCRequest
{
    [JsonProperty("jsonrpc")] public string JsonRpc { get; set; }
    [JsonProperty("method")] public string Method { get; set; }
    [JsonProperty("params")] public JToken Params { get; set; }
    [JsonProperty("id")] public string Id { get; set; }

    public bool IsValid() => !string.IsNullOrEmpty(Method);

    public JObject GetParamsObject()
    {
        if (Params is JObject obj) return obj;
        return new JObject();
    }
}

public class JsonRPCSuccessResponse
{
    [JsonProperty("jsonrpc")] public string JsonRpc { get; set; } = "2.0";
    [JsonProperty("id")] public string Id { get; set; }
    [JsonProperty("result")] public JToken Result { get; set; }

    public string ToJson() => JsonConvert.SerializeObject(this);
}

public class JsonRPCError
{
    [JsonProperty("code")] public int Code { get; set; }
    [JsonProperty("message")] public string Message { get; set; }
    [JsonProperty("data")] public JToken Data { get; set; }
}

public class JsonRPCErrorResponse
{
    [JsonProperty("jsonrpc")] public string JsonRpc { get; set; } = "2.0";
    [JsonProperty("id")] public string Id { get; set; }
    [JsonProperty("error")] public JsonRPCError Error { get; set; }

    public string ToJson() => JsonConvert.SerializeObject(this);
}

public static class JsonRPCErrorCodes
{
    public const int ParseError = -32700;
    public const int InvalidRequest = -32600;
    public const int MethodNotFound = -32601;
    public const int InvalidParams = -32602;
    public const int InternalError = -32603;
}
