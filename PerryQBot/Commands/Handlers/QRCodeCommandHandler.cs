using Mirai.Net.Data.Messages.Concretes;
using Mirai.Net.Utils.Scaffolds;
using QRCoder;
using Volo.Abp.DependencyInjection;

namespace PerryQBot.Commands.Handlers
{
    [Command("qrcode")]
    [ExposeServices(typeof(ICommandHandler))]
    public class QRCodeCommandHandler : CommandHandlerBase
    {
        public string QRCodeMessage { get; set; }

        public override async Task ExecuteAsync(CommandContext context)
        {
            QRCodeMessage = context.Message;
            await Task.CompletedTask;
        }

        public override MessageChainBuilder OnMessageChainBuilding(MessageChainBuilder builder)
        {
            var bytes = BitmapByteQRCodeHelper.GetQRCode(QRCodeMessage, QRCodeGenerator.ECCLevel.Q, 20);
            var base64 = Convert.ToBase64String(bytes);
            return builder.ImageFromBase64(base64);
        }
    }
}