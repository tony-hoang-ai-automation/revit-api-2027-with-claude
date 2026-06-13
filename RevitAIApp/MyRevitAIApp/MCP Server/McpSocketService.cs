using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Autodesk.Revit.UI;
using Newtonsoft.Json;
using MyRevitAIApp.McpServer.Sdk;
using MyRevitAIApp.McpServer.Sdk.JsonRpc;
using Serilog;
using MyRevitAIApp.McpServer.SayHello;
using MyRevitAIApp.McpServer.AiElementFilter;
using MyRevitAIApp.McpServer.AnalyzeModelStatistics;
using MyRevitAIApp.McpServer.ColorSplash;
using MyRevitAIApp.McpServer.CreateDimension;
using MyRevitAIApp.McpServer.CreateGrid;
using MyRevitAIApp.McpServer.CreateLevel;
using MyRevitAIApp.McpServer.CreateLineElement;
using MyRevitAIApp.McpServer.CreatePointElement;
using MyRevitAIApp.McpServer.CreateRoom;
using MyRevitAIApp.McpServer.CreateStructuralFramingSystem;
using MyRevitAIApp.McpServer.CreateSurfaceElement;
using MyRevitAIApp.McpServer.DeleteElement;
using MyRevitAIApp.McpServer.ExecuteCode;
using MyRevitAIApp.McpServer.ExportRoomData;
using MyRevitAIApp.McpServer.GetAvailableFamilyTypes;
using MyRevitAIApp.McpServer.GetCurrentViewElements;
using MyRevitAIApp.McpServer.GetCurrentViewInfo;
using MyRevitAIApp.McpServer.GetDocumentInfo;
using MyRevitAIApp.McpServer.GetMaterialQuantities;
using MyRevitAIApp.McpServer.GetSelectedElements;
using MyRevitAIApp.McpServer.OperateElement;
using MyRevitAIApp.McpServer.TagRooms;
using MyRevitAIApp.McpServer.TagWalls;

namespace MyRevitAIApp.McpServer;

public sealed class McpSocketService
{
    private static McpSocketService? _instance;
    private TcpListener? _listener;
    private Thread? _listenerThread;
    private bool _isRunning;
    private const int Port = 8080;
    private UIApplication? _uiApp;
    private readonly McpCommandRegistry _registry = new();
    private McpCommandExecutor? _executor;

    public static McpSocketService Instance => _instance ??= new McpSocketService();

    private McpSocketService() { }

    public bool IsRunning => _isRunning;

    public void Initialize(UIApplication uiApp)
    {
        _uiApp = uiApp;
        _executor = new McpCommandExecutor(_registry);
        RegisterAllCommands(uiApp);
        Log.Information("[MCP] Socket service initialized on port {Port}", Port);
    }

    private void RegisterAllCommands(UIApplication uiApp)
    {
        _registry.ClearCommands();

        // Test
        _registry.RegisterCommand(new SayHelloCommand(uiApp));

        // Access / Query
        _registry.RegisterCommand(new GetAvailableFamilyTypesCommand(uiApp));
        _registry.RegisterCommand(new GetCurrentViewElementsCommand(uiApp));
        _registry.RegisterCommand(new GetCurrentViewInfoCommand(uiApp));
        _registry.RegisterCommand(new GetSelectedElementsCommand(uiApp));
        _registry.RegisterCommand(new GetDocumentInfoCommand(uiApp));

        // Creation
        _registry.RegisterCommand(new CreatePointElementCommand(uiApp));
        _registry.RegisterCommand(new CreateLineElementCommand(uiApp));
        _registry.RegisterCommand(new CreateSurfaceElementCommand(uiApp));
        _registry.RegisterCommand(new CreateGridCommand(uiApp));
        _registry.RegisterCommand(new CreateLevelCommand(uiApp));
        _registry.RegisterCommand(new CreateRoomCommand(uiApp));
        _registry.RegisterCommand(new CreateStructuralFramingSystemCommand(uiApp));
        _registry.RegisterCommand(new CreateDimensionCommand(uiApp));

        // Annotation
        _registry.RegisterCommand(new TagWallsCommand(uiApp));
        _registry.RegisterCommand(new TagRoomsCommand(uiApp));

        // Element operations
        _registry.RegisterCommand(new AIElementFilterCommand(uiApp));
        _registry.RegisterCommand(new OperateElementCommand(uiApp));
        _registry.RegisterCommand(new DeleteElementCommand(uiApp));
        _registry.RegisterCommand(new ColorSplashCommand(uiApp));

        // Data extraction
        _registry.RegisterCommand(new AnalyzeModelStatisticsCommand(uiApp));
        _registry.RegisterCommand(new ExportRoomDataCommand(uiApp));
        _registry.RegisterCommand(new GetMaterialQuantitiesCommand(uiApp));

        // Dynamic code execution
        _registry.RegisterCommand(new ExecuteCodeCommand(uiApp));

        Log.Information("[MCP] Registered {Count} commands", _registry.GetRegisteredCommands().Count());
    }

    public void Start()
    {
        if (_isRunning) return;

        try
        {
            _isRunning = true;
            _listener = new TcpListener(IPAddress.Any, Port);
            _listener.Start();

            _listenerThread = new Thread(ListenForClients) { IsBackground = true };
            _listenerThread.Start();

            Log.Information("[MCP] Server started on port {Port}", Port);
        }
        catch (Exception ex)
        {
            _isRunning = false;
            Log.Error(ex, "[MCP] Failed to start server");
        }
    }

    public void Stop()
    {
        if (!_isRunning) return;

        try
        {
            _isRunning = false;
            _listener?.Stop();
            _listener = null;

            if (_listenerThread?.IsAlive == true)
                _listenerThread.Join(1000);

            Log.Information("[MCP] Server stopped");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[MCP] Error stopping server");
        }
    }

    private void ListenForClients()
    {
        try
        {
            while (_isRunning)
            {
                var client = _listener!.AcceptTcpClient();
                var clientThread = new Thread(HandleClient) { IsBackground = true };
                clientThread.Start(client);
            }
        }
        catch (SocketException) { }
        catch (Exception ex)
        {
            Log.Error(ex, "[MCP] Listener error");
        }
    }

    private void HandleClient(object? clientObj)
    {
        var tcpClient = (TcpClient)clientObj!;
        using var stream = tcpClient.GetStream();

        try
        {
            var buffer = new byte[8192];
            while (_isRunning && tcpClient.Connected)
            {
                int bytesRead;
                try { bytesRead = stream.Read(buffer, 0, buffer.Length); }
                catch (IOException) { break; }

                if (bytesRead == 0) break;

                var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                var response = ProcessRequest(message);

                var responseBytes = Encoding.UTF8.GetBytes(response);
                stream.Write(responseBytes, 0, responseBytes.Length);
            }
        }
        catch (Exception ex)
        {
            Log.Debug(ex, "[MCP] Client handler error");
        }
        finally
        {
            tcpClient.Close();
        }
    }

    private string ProcessRequest(string requestJson)
    {
        JsonRPCRequest? request = null;
        try
        {
            request = JsonConvert.DeserializeObject<JsonRPCRequest>(requestJson);
            if (request == null || !request.IsValid())
                return ErrorResponse(null, JsonRPCErrorCodes.InvalidRequest, "Invalid JSON-RPC request");

            return _executor!.Execute(request);
        }
        catch (JsonException)
        {
            return ErrorResponse(null, JsonRPCErrorCodes.ParseError, "Invalid JSON");
        }
        catch (Exception ex)
        {
            return ErrorResponse(request?.Id, JsonRPCErrorCodes.InternalError, $"Internal error: {ex.Message}");
        }
    }

    private static string ErrorResponse(string? id, int code, string message) =>
        new JsonRPCErrorResponse
        {
            Id = id,
            Error = new JsonRPCError { Code = code, Message = message }
        }.ToJson();
}
