using Volo.Abp.DependencyInjection;

public interface IUserCommandHandler
{
    bool IsCommand(string message);

    Task HandleAsync(UserCommandContext context);
}

public class HelpCommandHandler : IUserCommandHandler, ITransientDependency
{
    public ILogger<HelpCommandHandler> Logger { get; set; }

    public bool IsCommand(string message) => message.StartsWith(UserCommand.Help);

    public async Task HandleAsync(UserCommandContext context)
    {
        await Task.CompletedTask;

        Logger.LogInformation("收到帮助命令，如果没有请求API，说明代码没问题");
    }
}