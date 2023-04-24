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
    public IOptions<QRCodeOptions> QRCodeOptions { get; set; }
    public string ImageUrl { get; set; }

    public override async Task ExecuteAsync(CommandContext context)
    {
        ImageUrl = $"{QRCodeOptions.Value.QueryUrl}?data={context.Message.Trim()}";
        await Task.CompletedTask;
    }

    public override MessageChainBuilder OnMessageChainBuilding(MessageChainBuilder builder)
    {
        // 默认的回复
        base.OnMessageChainBuilding(builder);

        if (!string.IsNullOrEmpty(ImageUrl))
        {
            // 当是图片的时候，不带其他信息。
            builder.Clear();
            builder.ImageFromUrl(ImageUrl);
        }
        return builder;
    }
}