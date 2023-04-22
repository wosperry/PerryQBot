using Volo.Abp.DependencyInjection;

namespace PerryQBot.Commands.Handlers
{
    [Command("ai", WithPrefix = false)]
    [ExposeServices(typeof(ICommandHandler))]
    public class WakeKeywordCommandHandler : CommandHandlerBase
    {
        public override bool IsContinueAfterHandled => true;
    }
}