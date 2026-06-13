namespace MyRevitAIApp.McpServer.Sdk;

public interface ICommandRegistry
{
    void RegisterCommand(IRevitCommand command);
    bool TryGetCommand(string commandName, out IRevitCommand command);
    IEnumerable<IRevitCommand> GetRegisteredCommands();
    void ClearCommands();
}
