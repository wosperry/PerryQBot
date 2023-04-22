﻿using System.Reflection;

public static class CommandHandlerExtensions
{
    public static (bool isCommand, string commandString, string messageString) TryGetCommand<TCommand>(this TCommand handler, string message) where TCommand : ICommandHandler
    {
        var commandStrings = handler.GetCommandStrings(message);
        var commandString = commandStrings.FirstOrDefault(commandString => message.TrimStart().StartsWith(commandString, StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrEmpty(commandString))
        {
            return (false, null, message);
        }

        var messageString = message.Replace(commandString, "").TrimStart();
        return (true, commandString, messageString);
    }

    public static List<string> GetCommandStrings<TCommand>(this TCommand handler, string message) where TCommand : ICommandHandler
    {
        var result = new List<string>();
        var attributes = handler.GetType().GetCustomAttributes();
        if (handler.GetType().Namespace.StartsWith("Castle.Proxies"))
        {
            // Autofac真滴烦，把我类型代理掉了，拿不到特性
            var getTargetMethod = handler.GetType().GetMethod("DynProxyGetTarget");
            var targetObject = getTargetMethod.Invoke(handler, Array.Empty<object>());
            attributes = targetObject.GetType().GetCustomAttributes();
        }
        var commandAttribute = attributes.FirstOrDefault(a => a is CommandAttribute) as CommandAttribute;
        if (!string.IsNullOrWhiteSpace(commandAttribute?.Command))
        {
            result.Add(commandAttribute?.Command);
        }
        return result;
    }
}