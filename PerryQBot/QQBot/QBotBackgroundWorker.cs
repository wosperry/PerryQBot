using System.Reactive.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Concretes;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Utils.Scaffolds;
using PerryQBot.Commands;
using PerryQBot.EntityFrameworkCore.Entities;
using PerryQBot.OpenAI;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Domain.Repositories;

namespace PerryQBot.QQBot;

public class QBotBackgroundWorker : BackgroundWorkerBase
{
    public IBackgroundJobManager JobManager { get; set; }
    public IOptions<MiraiBotOptions> BotOptions { get; set; }
    public IEnumerable<ICommandHandler> CommandHandlers { get; set; }
    public IOpenAIMessageManager OpenAIRequestManager { get; set; }
    public IRepository<User> UserRepository { get; set; }

    public QBotBackgroundWorker(IEnumerable<ICommandHandler> commandHandlers)
    {
        CommandHandlers = commandHandlers;
    }

    public MiraiBot Bot { get; set; }

    public override async Task StartAsync(CancellationToken cancellationToken = default)
    {
        Bot.MessageReceived.SubscribeGroupMessage(OnGroupMessageReceived);
        Bot.MessageReceived.SubscribeFriendMessage(OnFriendMessageReceived);

        await Bot.LaunchAsync();
        await MessageManager.SendFriendMessageAsync(BotOptions.Value.AdminQQ, new PlainMessage("机器人启动成功"));
    }

    private async void OnGroupMessageReceived(GroupMessageReceiver messageReceiver)
    {
        // 写死，检查敏感词并警告
        try
        {
            await VpnMessageCheckAsync(messageReceiver.Sender.Id, messageReceiver.Sender.Name, messageReceiver.MessageChain.GetPlainMessage());
        }
        catch (WarningException ex)
        {
            var b = new MessageChainBuilder()
                .At(messageReceiver.Sender.Id)
                .Plain(" ")
                .Plain(ex.Message);
            await MessageManager.SendGroupMessageAsync(messageReceiver.GroupId, b.Build());

            return;
        }

        // 默认Handle到了，就结束，除非设置 IsContinueAfterHandled=true
        var commandHandler = await HandleCommandAsync(messageReceiver);
        if (commandHandler is not null && !commandHandler.IsContinueAfterHandled) return;

        var isAt = messageReceiver.MessageChain.Any(t => t is AtMessage at && at.Target == Bot.QQ);
        var isCommandContinue = commandHandler?.IsContinueAfterHandled ?? false;
        if (isAt || isCommandContinue)
        {
            var userMessage = (commandHandler is not null) ? commandHandler.RequestMessage : messageReceiver.MessageChain.GetPlainMessage();

            Logger.LogInformation("{friendName}在群【{groupName}】@我：{plainText}", messageReceiver.Sender.Name, messageReceiver.GroupName, userMessage);
            // 编辑消息链，预设和历史
            var messages = await OpenAIRequestManager.BuildUserRequestMessagesAsync(messageReceiver.Sender.Id, messageReceiver.Sender.Name, userMessage);

            // 入队，发请求
            await JobManager.EnqueueAsync(new OpenAIRequestingBackgroundJobArgs
            {
                SenderId = messageReceiver.Sender.Id,
                SenderName = messageReceiver.Sender.Name,
                GroupId = messageReceiver.GroupId,
                GroupName = messageReceiver.GroupName,
                Type = MessageReceivers.Group,
                Messages = messages
            });
        }
    }

    private async void OnFriendMessageReceived(FriendMessageReceiver messageReceiver)
    {
        // Handle到了，就结束
        var commandHandler = await HandleCommandAsync(messageReceiver);
        if (commandHandler is not null && !commandHandler.IsContinueAfterHandled) return;

        // 编辑消息链，预设和历史
        var userMessage = (commandHandler is not null) ? commandHandler.RequestMessage : messageReceiver.MessageChain.GetPlainMessage();
        Logger.LogInformation("{friendName} 私聊我：{qqMessage}", messageReceiver.FriendName, userMessage);
        var messages = await OpenAIRequestManager.BuildUserRequestMessagesAsync(messageReceiver.FriendId, messageReceiver.FriendName, userMessage);
        await JobManager.EnqueueAsync(new OpenAIRequestingBackgroundJobArgs
        {
            SenderId = messageReceiver.Sender.Id,
            SenderName = messageReceiver.FriendName,
            Type = MessageReceivers.Friend,
            Messages = messages
        });
    }

    private async Task<ICommandHandler> HandleCommandAsync(MessageReceiverBase messageReceiver)
    {
        foreach (var handler in CommandHandlers)
        {
            var (isCommand, commandString, messageString) = handler.TryGetCommand(messageReceiver.MessageChain.GetPlainMessage(), BotOptions.Value.CommandPrefix);
            if (isCommand)
            {
                var context = new CommandContext
                {
                    Type = messageReceiver.Type,
                    Message = messageString,
                    CommandString = commandString,
                    MessageChain = messageReceiver.MessageChain
                };

                if (messageReceiver is GroupMessageReceiver groupMessage)
                {
                    context.SenderId = groupMessage.Sender.Id;
                    context.SenderName = groupMessage.Sender.Name;
                    context.GroupId = groupMessage.GroupId;
                    context.GroupName = groupMessage.GroupName;
                }

                if (messageReceiver is FriendMessageReceiver friendMessage)
                {
                    context.SenderId = friendMessage.FriendId;
                    context.SenderName = friendMessage.FriendName;
                }

                // 执行命令
                handler.RequestMessage = context.Message;
                await handler.HandleAsync(context);
                return handler;
            }
        }

        return null;
    }

    private async Task VpnMessageCheckAsync(string qq, string name, string message)
    {
        if (message.Contains("vpn", StringComparison.OrdinalIgnoreCase) ||
            message.Contains("vpn", StringComparison.OrdinalIgnoreCase)
            )
        {
            var user = await (await UserRepository.GetQueryableAsync())
                .Where(t => t.QQ == qq).FirstOrDefaultAsync();
            if (user is null)
            {
                user = new User()
                {
                    QQ = qq,
                    QQNickName = name,
                    WarnTime = 1
                };
                await UserRepository.InsertAsync(user, true);
            }
            else
            {
                user.WarnTime += 1;
                await UserRepository.UpdateAsync(user, true);
            }

            throw new WarningException($"警告一次，用户{user.QQ}，已违规{user.WarnTime}次");
        }
    }
}