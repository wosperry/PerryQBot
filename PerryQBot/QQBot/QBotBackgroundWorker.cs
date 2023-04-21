using System.Reactive.Linq;
using Mirai.Net.Data.Events.Concretes.Request;
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
    public MiraiBot Bot { get; }
    public IBackgroundJobManager JobManager { get; set; }

    public QBotBackgroundWorker(MiraiBot bot)
    {
        Bot = bot;
    }

    public override async Task StartAsync(CancellationToken cancellationToken = default)
    {
        Bot.MessageReceived.SubscribeGroupMessage(OnGroupMessageReceived);
        Bot.MessageReceived.SubscribeFriendMessage(OnFriendMessageReceived);

        await Bot.LaunchAsync();
        await MessageManager.SendFriendMessageAsync("593281239", new PlainMessage("机器人启动成功"));
    }

    private async void OnGroupMessageReceived(GroupMessageReceiver message)
    {
        if (message.MessageChain.Any(t => (t is AtMessage at) && at.Target == Bot.QQ))
        {
            var friendName = message.Sender.Name;
            var groupName = message.GroupName;
            var plainText = message.MessageChain.GetPlainMessage();
            Logger.LogInformation("{friendName}在群【{groupName}】@我：{plainText}", friendName, groupName, plainText);

            await JobManager.EnqueueAsync(new OpenAIRequestingBackgroundJobArgs
            {
                SenderId = message.Sender.Id,
                SenderName = message.Sender.Name,
                GroupId = message.GroupId,
                GroupName = message.GroupName,
                Type = MessageReceivers.Group,
                // TODO: 加预设和历史
                Messages = new List<string>
                {
                    message.MessageChain.GetPlainMessage()
                }
            });
        }
    }

    private async void OnFriendMessageReceived(FriendMessageReceiver message)
    {
        var qqMessage = message.MessageChain.GetPlainMessage();
        var friendName = message.FriendName;
        Logger.LogInformation("{friendName} 私聊我：{qqMessage}", friendName, qqMessage);

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
}