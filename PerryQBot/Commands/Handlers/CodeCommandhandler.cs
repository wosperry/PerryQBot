using Volo.Abp.DependencyInjection;

namespace PerryQBot.Commands.Handlers;

[Command("代码")]
[ExposeServices(typeof(ICommandHandler))]
public class CodeCommandhandler : CommandHandlerBase
{
    public override async Task<string> HandleAndResponseAsync(CommandContext context)
    {
        await Task.CompletedTask;
        return "https://github.com/wosperry/PerryQBot";
    }
}