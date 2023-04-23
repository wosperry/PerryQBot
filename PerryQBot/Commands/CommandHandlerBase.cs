using Microsoft.Extensions.Options;
using Mirai.Net.Data.Messages;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Utils.Scaffolds;
using PerryQBot.Commands.Handlers;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;

namespace PerryQBot.Commands;

public abstract class CommandHandlerBase : ICommandHandler, ITransientDependency
{
    public virtual bool IsContinueAfterHandled { get; set; } = false;
    public virtual string ResponseMessage { get; set; }
    public virtual string RequestMessage { get; set; }
    public ILogger<HelpCommandHandler> Logger { get; set; }
    public IOptions<MiraiBotOptions> BotOptions { get; set; }
    public IServiceProvider ServiceProvider { get; set; }

    public virtual async Task ExecuteAsync(CommandContext context)
    {
        await Task.CompletedTask;
    }

    public Task SendMessageToAdminAsync(string message) => MessageManager.SendFriendMessageAsync(BotOptions.Value.AdminQQ, message);

    [UnitOfWork]
    public virtual async Task HandleAsync(CommandContext context)
    {
        try
        {
            await ExecuteAsync(context);
            if (!IsContinueAfterHandled)
            {
                if (context.Type == MessageReceivers.Friend)
                {
                    var messageChainBuilder = OnMessageChainBuilding(new MessageChainBuilder());
                    var messageChain = messageChainBuilder.Build();
                    await MessageManager.SendFriendMessageAsync(context.SenderId, messageChain);
                }
                if (context.Type == MessageReceivers.Group)
                {
                    var messageChainBuilder = OnMessageChainBuilding(new MessageChainBuilder().At(context.SenderId).Plain(" "));
                    var messageChain = messageChainBuilder.Build();
                    await MessageManager.SendGroupMessageAsync(context.GroupId, messageChain);
                }
            }
        }
        catch (Exception ex)
        {
            var errorMessage = $"""
                ----------------
                【{context.SenderName}({context.SenderId})】命令处理失败：
                命令：{context.CommandString}
                错误：{ex.Message}
                ----------------
                """;
            await SendMessageToAdminAsync(errorMessage);
            Logger.LogError(errorMessage);
        }
    }

    public virtual MessageChainBuilder OnMessageChainBuilding(MessageChainBuilder builder)
    {
        return builder.Plain(ResponseMessage);
    }
}