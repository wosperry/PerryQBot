using Mirai.Net.Data.Messages;
using Mirai.Net.Sessions.Http.Managers;
using Volo.Abp.DependencyInjection;

public class HelpCommandHandler : CommandHandlerBase, ITransientDependency
{
    public override string GetCommand() => "#帮助";

    public override async Task HandleAsync(CommandContext context)
    {
        if (context.Type == MessageReceivers.Friend)
        {
            await MessageManager.SendFriendMessageAsync(context.SenderId, "这是一段帮助信息");
        }
    }
}