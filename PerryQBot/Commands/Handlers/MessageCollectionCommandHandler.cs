using Microsoft.Extensions.Logging;
using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Concretes;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions.Http.Managers;
using Newtonsoft.Json;
using PerryQBot.EntityFrameworkCore.Entities;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace PerryQBot.Commands.Handlers
{
    [Command("收藏消息")]
    [ExposeServices(typeof(ICommandHandler))]
    public class MessageCollectionCommandHandler : CommandHandlerBase
    {
        public IRepository<DialogCollection> DialogCollectionRepository { get; set; }

        public override async Task ExecuteAsync(CommandContext context)
        {
            var quoteMessage = context.MessageChain.FirstOrDefault(t => t is QuoteMessage) as QuoteMessage;

            var origin = quoteMessage.Origin;

            // 只处理群聊的消息
            if (context.Type == MessageReceivers.Group)
            {
                var groupMessageReceiver = await MessageManager.GetMessageReceiverByIdAsync<GroupMessageReceiver>(quoteMessage.MessageId, context.GroupId);

                // 保存消息
                // TODO: 有个问题，如果是其他类型消息怎么处理，比如图片，语音，表情等
                var groupPlainMessage = groupMessageReceiver.MessageChain.GetPlainMessage();

                var dialogCollection = new DialogCollection
                {
                    Message = context.Message,
                    UserQQ = context.SenderId,
                    UserName = context.SenderName,
                    DateTime = DateTime.Now,
                    QuoteMessage = JsonConvert.SerializeObject(origin.Select(x => (x as PlainMessage)?.Text).Where(x => !string.IsNullOrEmpty(x)).ToList())
                };
                await DialogCollectionRepository.InsertAsync(dialogCollection);
                Logger.LogInformation(JsonConvert.SerializeObject(dialogCollection));
                ResponseMessage = "收藏成功";
                Logger.LogInformation("收藏成功：{obj}", JsonConvert.SerializeObject(dialogCollection));
            }
        }
    }
}