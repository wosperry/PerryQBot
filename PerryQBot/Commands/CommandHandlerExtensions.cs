using System.Reflection;

public static class CommandHandlerExtensions
{
    public static bool IsCommand<TCommand>(this TCommand handler, string message) where TCommand : ICommandHandler
    {
        if (typeof(TCommand).IsDefined(typeof(CommandAttribute), false))
        {
            var command = typeof(TCommand).GetCustomAttribute<CommandAttribute>();
            return message.TrimStart().StartsWith(command.Command);
        }
        return false;
    }
}