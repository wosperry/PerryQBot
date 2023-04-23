using Volo.Abp.DependencyInjection;

namespace PerryQBot.Commands.Handlers;

[Command("qrcode")]
[Command("二维码")]
[ExposeServices(typeof(ICommandHandler))]
public class QRCodeCommandHandler : CommandHandlerBase
{
    public override async Task ExecuteAsync(CommandContext context)
    {
        await Task.CompletedTask;
    }
}