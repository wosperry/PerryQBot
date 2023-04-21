using System.Reflection;

public static class CommandHandlerExtensions
{
    public static bool IsCommand<TCommand>(this TCommand handler, string message) where TCommand : ICommandHandler
    {
        return message.TrimStart().StartsWith(handler.GetCommandString(message));
    }

    public static string GetCommandString<TCommand>(this TCommand handler, string message) where TCommand : ICommandHandler
    {
        var attributes = handler.GetType().GetCustomAttributes();
        var commandAttribute = attributes.FirstOrDefault(a => a is CommandAttribute) as CommandAttribute;

        return $"#{commandAttribute?.Command ?? ""}";
    }

    public static string GetMessageString<TCommand>(this TCommand handler, string message) where TCommand : ICommandHandler
    {
        var commandString = handler.GetCommandString(message);
        return message.Replace(commandString, "").TrimStart();
    }
}