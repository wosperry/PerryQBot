using PerryQBot.Commands;

namespace PerryQBot.CommandHandlers;

[Command("解析二维码")]
[Command("qrdecode")]
public class QRDecodeCommandHandler : CommandHandlerBase
{
    public override async Task ExecuteAsync(CommandContext context)
    {
        await Task.CompletedTask;
        ResponseMessage = "请发送图片";
    }
}