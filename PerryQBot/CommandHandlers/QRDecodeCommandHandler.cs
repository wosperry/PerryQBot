using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;
using Mirai.Net.Data.Messages.Concretes;
using Mirai.Net.Utils.Scaffolds;
using PerryQBot.Commands;
using PerryQBot.Options;

namespace PerryQBot.CommandHandlers;

[Command("解析二维码")]
[Command("qrdecode")]
public class QRDecodeCommandHandler : CommandHandlerBase
{
    public IOptions<Apis> Apis { get; set; }
    public List<ImageMessage> ImageMessages { get; set; } = new();

    public override async Task ExecuteAsync(CommandContext context)
    {
        await Task.CompletedTask;
        foreach (var m in context.MessageChain)
        {
            if (m is ImageMessage imageMessage)
            {
                ImageMessages.Add(imageMessage);
            }
        }
    }

    public override MessageChainBuilder OnMessageChainBuilding(MessageChainBuilder builder)
    {
        if (ImageMessages.Count > 0)
        {
            foreach (var imageMessage in ImageMessages)
            {
                // 调接口解析
                var url = new Url($"{Apis.Value.UomgAPI}/qr.encode")
                    .SetQueryParam("url", imageMessage.Url);

                var result = url.GetJsonAsync().Result;
                if (result.code == 1)
                {
                    // 放一个图片
                    builder.Append(imageMessage);

                    // 放上解析后的字符串
                    builder.Plain($"内容：{result.qrurl}");
                }
            }
        }
        else
        {
            ResponseMessage = "请发送图片";
        }

        return builder;
    }
}