using MyRevitAIApp.McpServer.Sdk;

namespace MyRevitAIApp.McpServer;

public sealed class McpCommandRegistry : ICommandRegistry
{
    private readonly Dictionary<string, IRevitCommand> _commands = new();

    public void RegisterCommand(IRevitCommand command) =>
        _commands[command.CommandName] = command;

    public bool TryGetCommand(string commandName, out IRevitCommand command) =>
        _commands.TryGetValue(commandName, out command!);

    public IEnumerable<IRevitCommand> GetRegisteredCommands() => _commands.Values;

    public void ClearCommands() => _commands.Clear();
}
