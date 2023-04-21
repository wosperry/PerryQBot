public interface ICommandHandler
{
    public abstract string GetCommand();

    bool IsCommand(string message);

    Task HandleAsync(CommandContext context);
}