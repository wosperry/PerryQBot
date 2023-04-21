using Volo.Abp.DependencyInjection;

public interface ICommandHandler
{
    bool IsCommand(string message);

    Task HandleAsync(CommandContext context);
}

public class HelpCommandHandler : ICommandHandler, ITransientDependency
{
    public ILogger<HelpCommandHandler> Logger { get; set; }

    public bool IsCommand(string message) => message.StartsWith(Command.Help);

    public async Task HandleAsync(CommandContext context)
    {
        await Task.CompletedTask;

        Logger.LogInformation("收到帮助命令，如果没有请求API，说明代码没问题");
    }
}