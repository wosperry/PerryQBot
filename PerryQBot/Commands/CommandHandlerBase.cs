using Mirai.Net.Data.Messages;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Utils.Scaffolds;

public abstract class CommandHandlerBase : ICommandHandler
{
    public ILogger<HelpCommandHandler> Logger { get; set; }

    public abstract string HandleAndResponseAsync(CommandContext context);

    public async Task HandleAsync(CommandContext context)
    {
        var message = HandleAndResponseAsync(context);
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
}