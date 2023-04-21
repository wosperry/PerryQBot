using System.Reactive.Linq;
using Microsoft.Extensions.Options;
using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Concretes;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Utils.Scaffolds;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BackgroundWorkers;

public class QBotBackgroundWorker : BackgroundWorkerBase
{
    public IBackgroundJobManager JobManager { get; set; }
    public IOptions<MiraiBotOptions> BotOptions { get; set; }
    public MiraiBot Bot { get; set; }
    public IEnumerable<IUserCommandHandler> CommandHandlers { get; set; }

    public override async Task StartAsync(CancellationToken cancellationToken = default)
    {
        Bot.MessageReceived.SubscribeGroupMessage(OnGroupMessageReceived);
        Bot.MessageReceived.SubscribeFriendMessage(OnFriendMessageReceived);

        await Bot.LaunchAsync();
        await MessageManager.SendFriendMessageAsync(BotOptions.Value.AdminQQ, new PlainMessage("机器人启动成功"));
    }

    private async void OnGroupMessageReceived(GroupMessageReceiver message)
    {
        if (message.MessageChain.Any(t => (t is AtMessage at) && at.Target == Bot.QQ))
        {
            Logger.LogInformation("{friendName}在群【{groupName}】@我：{plainText}", message.Sender.Name, message.GroupName, message.MessageChain.GetPlainMessage());

            // Handle到了，就结束
            if (await HandleUserCommandAsync(message)) return;

            // TODO: 加预设和历史
            var messages = new List<string>
            {
                message.MessageChain.GetPlainMessage()
            };

            // 入队，发请求
            await JobManager.EnqueueAsync(new OpenAIRequestingBackgroundJobArgs
            {
                SenderId = message.Sender.Id,
                SenderName = message.Sender.Name,
                GroupId = message.GroupId,
                GroupName = message.GroupName,
                Type = MessageReceivers.Group,
                Messages = messages
            });
        }
    }

    private async void OnFriendMessageReceived(FriendMessageReceiver message)
    {
        Logger.LogInformation("{friendName} 私聊我：{qqMessage}", message.MessageChain.GetPlainMessage(), message.FriendName);

        // Handle到了，就结束
        if (await HandleUserCommandAsync(message)) return;

        await JobManager.EnqueueAsync(new OpenAIRequestingBackgroundJobArgs
        {
            SenderId = message.Sender.Id,
            SenderName = message.Sender.NickName,
            Type = MessageReceivers.Friend,
            // TODO: 加预设和历史
            Messages = new List<string>
            {
                message.MessageChain.GetPlainMessage()
            }
        });
    }

    private async Task<bool> HandleUserCommandAsync(MessageReceiverBase message)
    {
        foreach (var handler in CommandHandlers)
        {
            if (handler.IsCommand(message.MessageChain.GetPlainMessage()))
            {
                var context = new UserCommandContext
                {
                    Type = message.Type,
                    Message = message.MessageChain.GetPlainMessage(),
                };

                if (message is GroupMessageReceiver groupMessage)
                {
                    context.SenderId = groupMessage.Sender.Id;
                    context.SenderName = groupMessage.Sender.Name;
                    context.GroupId = groupMessage.GroupId;
                    context.GroupName = groupMessage.GroupName;
                }

                if (message is FriendMessageReceiver friendMessage)
                {
                    context.SenderId = friendMessage.FriendId;
                    context.SenderName = friendMessage.FriendName;
                }

                // 执行命令
                await handler.HandleAsync(context);
                return true;
            }
        }
        return false;
    }
}