using Mirai.Net.Utils.Scaffolds;
using Volo.Abp.DependencyInjection;

namespace PerryQBot.Commands.Handlers
{
    [Command("qrcode")]
    [ExposeServices(typeof(ICommandHandler))]
    public class QRCodeCommandHandler : CommandHandlerBase
    {
        public override MessageChainBuilder OnMessageChainBuilding(MessageChainBuilder builder)
        {
            // 不执行默认追加的原始消息
            // return base.OnMessageChainBuilding(builder);
        }
    }
}