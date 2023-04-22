using System.Reflection;

public static class CommandHandlerExtensions
{
    public static bool IsCommand<TCommand>(this TCommand handler, string message) where TCommand : ICommandHandler
    {
        var commandString = handler.GetCommandString(message);

        if (string.IsNullOrWhiteSpace(commandString))
        {
            return false;
        }
        return message.TrimStart().StartsWith(commandString, StringComparison.OrdinalIgnoreCase);
    }

    public static string GetCommandString<TCommand>(this TCommand handler, string message) where TCommand : ICommandHandler
    {
        var result = "";
        if (handler.GetType().Namespace.StartsWith("Castle.Proxies"))
        {
            // Autofac真滴烦，把我类型代理掉了，拿不到特性
            var getTargetMethod = handler.GetType().GetMethod("DynProxyGetTarget");
            var targetObject = getTargetMethod.Invoke(handler, Array.Empty<object>());
            var attributes = targetObject.GetType().GetCustomAttributes(); ;
            var commandAttribute = attributes.FirstOrDefault(a => a is CommandAttribute) as CommandAttribute;
            result = commandAttribute?.Command ?? "";
        }
        else
        {
            var attributes = handler.GetType().GetCustomAttributes();
            var commandAttribute = attributes.FirstOrDefault(a => a is CommandAttribute) as CommandAttribute;
            result = commandAttribute?.Command ?? "";
        }
        return string.IsNullOrWhiteSpace(result) ? "" : ("#" + result);
    }

    public static string GetMessageString<TCommand>(this TCommand handler, string message) where TCommand : ICommandHandler
    {
        var commandString = handler.GetCommandString(message);
        return message.Replace(commandString, "").TrimStart();
    }
}