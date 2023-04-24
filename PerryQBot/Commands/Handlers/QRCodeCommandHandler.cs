using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;
using Mirai.Net.Utils.Scaffolds;
using PerryQBot.Options;
using Volo.Abp.DependencyInjection;

namespace PerryQBot.Commands.Handlers;

[Command("qrcode")]
[Command("二维码")]
[ExposeServices(typeof(ICommandHandler))]
public class QRCodeCommandHandler : CommandHandlerBase
{
    public IOptions<Apis> Apis { get; set; }
    public string ImageUrl { get; set; }

    public override async Task ExecuteAsync(CommandContext context)
    {
        ImageUrl = $"{Apis.Value.UomgAPI}/qrcode?url={context.Message.Trim()}";
        await Task.CompletedTask;
    }

    public override MessageChainBuilder OnMessageChainBuilding(MessageChainBuilder builder) => builder.ImageFromUrl(ImageUrl);
}