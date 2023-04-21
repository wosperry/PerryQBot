public interface ICommandHandler
{
    Task HandleAsync(CommandContext context);
}