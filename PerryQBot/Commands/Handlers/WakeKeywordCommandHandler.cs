using Volo.Abp.DependencyInjection;

namespace PerryQBot.Commands.Handlers
{
    [Command("ai", WithPrefix = false)]
    [Command("喵", WithPrefix = false)]
    [Command("呐", WithPrefix = false)]
    [ExposeServices(typeof(ICommandHandler))]
    public class WakeKeywordCommandHandler : CommandHandlerBase
    {
        public override bool IsContinueAfterHandled => true;
    }
}