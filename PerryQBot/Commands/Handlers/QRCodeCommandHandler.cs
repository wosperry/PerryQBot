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
    public string ImageBase64 { get; set; }

    public override async Task ExecuteAsync(CommandContext context)
    {
        var url = new Url(QRCodeOptions.Value.QueryUrl);

        var result = await url
            .SetQueryParam("data", context.Message)
            .GetJsonAsync();
        if (result.code == 0)
        {
            ResponseMessage = context.Message;
            ImageBase64 = result.result.base64_image;
        }
        else
        {
            ResponseMessage = "生成失败";
        }
    }

    public override MessageChainBuilder OnMessageChainBuilding(MessageChainBuilder builder)
    {
        // 默认的回复
        base.OnMessageChainBuilding(builder);

        if (!string.IsNullOrEmpty(ImageBase64))
        {
            // 当是图片的时候，不带其他信息。
            builder.Clear();
            builder.ImageFromBase64(ImageBase64);
        }
        return builder;
    }
}