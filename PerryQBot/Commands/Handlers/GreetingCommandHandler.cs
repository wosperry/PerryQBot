using Volo.Abp.DependencyInjection;

namespace PerryQBot.Commands.Handlers;

[Command("你好")]
[ExposeServices(typeof(ICommandHandler))]
public class GreetingCommandHandler : CommandHandlerBase
{
    public override async Task<string> HandleAndResponseAsync(CommandContext context)
    {
        await Task.CompletedTask;

        return $"你好，{context.SenderName}";
    }
}