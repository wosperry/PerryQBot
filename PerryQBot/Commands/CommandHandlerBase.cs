using Microsoft.Extensions.Options;
using Mirai.Net.Data.Messages;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Utils.Scaffolds;
using Newtonsoft.Json;
using PerryQBot.Commands.Handlers;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;

public abstract class CommandHandlerBase : ICommandHandler, ITransientDependency
{
    public ILogger<HelpCommandHandler> Logger { get; set; }
    public IOptions<MiraiBotOptions> BotOptions { get; set; }
    public IServiceProvider ServiceProvider { get; set; }

    public abstract Task<string> HandleAndResponseAsync(CommandContext context);

    public Task SendMessageToAdminAsync(string message) => MessageManager.SendFriendMessageAsync(BotOptions.Value.AdminQQ, message);

    [UnitOfWork]
    public virtual async Task HandleAsync(CommandContext context)
    {
        try
        {
            var message = await HandleAndResponseAsync(context);

            if (context.Type == MessageReceivers.Friend)
            {
                await MessageManager.SendFriendMessageAsync(context.SenderId, message);
            }
            if (context.Type == MessageReceivers.Group)
            {
                var messageChain = new MessageChainBuilder()
                    .At(context.SenderId)
                    .Plain(" ")
                    .Plain(message)
                    .Build();
                await MessageManager.SendGroupMessageAsync(context.GroupId, messageChain);
            }
        }
        catch (Exception ex)
        {
            await SendMessageToAdminAsync(JsonConvert.SerializeObject(new
            {
                context.SenderId,
                context.SenderName,
                Command = context.CommandString,
                ErrorMessage = "任务执行失败，请查看系统日志。"
            }));
            Logger.LogError("任务执行失败{message}", ex.Message);
        }
    }
}