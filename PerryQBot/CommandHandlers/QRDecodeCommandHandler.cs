using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;
using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Concretes;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Utils.Scaffolds;
using PerryQBot.Commands;
using PerryQBot.Options;

namespace PerryQBot.CommandHandlers;

[Command("解析")]
[Command("解析二维码")]
[Command("qrdecode")]
public class QRDecodeCommandHandler : CommandHandlerBase
{
    public IOptions<Apis> Apis { get; set; }
    public ImageMessage ImageMessage { get; set; }

    public override async Task ExecuteAsync(CommandContext context)
    {
        await Task.CompletedTask;

        // 优先处理引用消息
        var quoteMessage = context.MessageChain.FirstOrDefault(t => t is QuoteMessage) as QuoteMessage;
        if (quoteMessage is not null)
        {
            var quoteMessageReceiver = await MessageManager.GetMessageReceiverByIdAsync<MessageReceiverBase>(quoteMessage.MessageId,
                context.Type == MessageReceivers.Friend ? context.SenderId : context.GroupId);
            if (quoteMessageReceiver is not null)
            {
                // TODO: 有个问题，如果是其他类型消息怎么处理，比如图片，语音，表情等
                ImageMessage = quoteMessageReceiver.MessageChain.FirstOrDefault(t => t is ImageMessage) as ImageMessage;
                if (ImageMessage is not null)
                {
                    return;
                }
            }
        }

        // 处理图片消息
        ImageMessage = context.MessageChain.FirstOrDefault(t => t is ImageMessage) as ImageMessage;
    }

    public override MessageChainBuilder OnMessageChainBuilding(MessageChainBuilder builder)
    {
        if (ImageMessage is null)

        {
            ResponseMessage = "请发送图片";
        }
        else
        {
            // 调接口解析
            var url = new Url($"{Apis.Value.UomgAPI}/qr.encode")
                .SetQueryParam("url", ImageMessage.Url);
            var result = url.GetJsonAsync().Result;
            if (result.code == 1)
            {
                // 放上解析后的字符串
                builder.Plain($"内容：{result.qrurl}");
            }
        }

        return builder;
    }
}