using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Concretes;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions.Http.Managers;
using Newtonsoft.Json;
using PerryQBot.Commands;
using PerryQBot.EntityFrameworkCore.Entities;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace PerryQBot.CommandHandlers;

[Command("收藏")]
[ExposeServices(typeof(ICommandHandler))]
public class MessageCollectionCommandHandler : CommandHandlerBase
{
    public IRepository<DialogCollection> DialogCollectionRepository { get; set; }

    public override async Task ExecuteAsync(CommandContext context)
    {
        // 只处理群聊的消息
        if (context.Type == MessageReceivers.Group)
        {
            var dialogCollection = new DialogCollection
            {
                Message = context.Message,
                UserQQ = context.SenderId,
                UserName = context.SenderName,
                DateTime = DateTime.Now,
            };

            // 处理引用消息
            var quoteMessage = context.MessageChain.FirstOrDefault(t => t is QuoteMessage) as QuoteMessage;
            if (quoteMessage is not null)
            {
                var quoteMessageReceiver = await MessageManager.GetMessageReceiverByIdAsync<GroupMessageReceiver>(quoteMessage.MessageId, context.GroupId);
                if (quoteMessageReceiver is not null)
                {
                    // TODO: 有个问题，如果是其他类型消息怎么处理，比如图片，语音，表情等
                    var groupPlainMessage = quoteMessageReceiver.MessageChain.GetPlainMessage();
                    if (!string.IsNullOrWhiteSpace(groupPlainMessage))
                    {
                        dialogCollection.QuoteMessage = groupPlainMessage;
                    }
                }
            }

            // 保存消息
            await DialogCollectionRepository.InsertAsync(dialogCollection);
            // 设置回复消息
            ResponseMessage = "收藏成功";
            Logger.LogInformation("收藏成功：{obj}", JsonConvert.SerializeObject(dialogCollection));
        }
    }
}