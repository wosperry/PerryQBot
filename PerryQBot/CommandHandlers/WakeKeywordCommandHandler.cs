using PerryQBot.Commands;
using Volo.Abp.DependencyInjection;

namespace PerryQBot.CommandHandlers;

[Command("ai", false)]
[Command("喵", false)]
[Command("呐", false)]
[ExposeServices(typeof(ICommandHandler))]
public class WakeKeywordCommandHandler : CommandHandlerBase
{
    public override bool IsContinueAfterHandled => true;
}