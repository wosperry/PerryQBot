using Volo.Abp.DependencyInjection;

namespace PerryQBot.Commands.Handlers;

[Command("ai", false)]
[Command("喵", false)]
[Command("呐", false)]
[ExposeServices(typeof(ICommandHandler))]
public class WakeKeywordCommandHandler : CommandHandlerBase
{
    public override bool IsContinueAfterHandled => true;
}