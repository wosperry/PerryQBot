using System.Reflection;

public static class CommandHandlerExtensions
{
    public static bool IsCommand<TCommand>(this TCommand handler, string message) where TCommand : ICommandHandler
    {
        var attributes = typeof(TCommand).GetCustomAttributes();
        var commandAttribute = attributes.FirstOrDefault(a => a is CommandAttribute) as CommandAttribute;
        if (commandAttribute is null)
        {
            return false;
        }

        return message.TrimStart().StartsWith($"#{commandAttribute.Command}");
    }
}