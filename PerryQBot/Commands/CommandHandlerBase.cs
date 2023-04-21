public abstract class CommandHandlerBase : ICommandHandler
{
    public ILogger<HelpCommandHandler> Logger { get; set; }

    public abstract string GetCommand();

    public abstract Task HandleAsync(CommandContext context);

    public bool IsCommand(string message)
    {
        return message.StartsWith(GetCommand());
    }
}
