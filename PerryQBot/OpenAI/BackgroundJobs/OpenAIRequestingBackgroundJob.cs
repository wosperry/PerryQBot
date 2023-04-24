using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;
using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Concretes;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Utils.Scaffolds;
using PerryQBot.EntityFrameworkCore.Entities;
using PerryQBot.Options;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Domain.Repositories;

public class OpenAIRequestingBackgroundJob : BackgroundJob<OpenAIRequestingBackgroundJobArgs>, ITransientDependency
{
    public IOptions<OpenAiOptions> OpenAiOptions { get; set; }
    public IOptions<MiraiBotOptions> BotOptions { get; set; }
    public IAbpDistributedLock DistributedLock { get; set; }
    public IRepository<UserHistory> UserHistoryRepository { get; set; }

    public IReadOnlyRepository<User> UserRepository { get; set; }

    private static readonly object _lock = new object();

    // TODO: 考虑发消息改到一个独立的后台任务中

    public override async void Execute(OpenAIRequestingBackgroundJobArgs args)
    {
        var url = new Url(new Uri(OpenAiOptions.Value.CompletionUrl))
            .WithHeader("Authorization", $"Bearer {OpenAiOptions.Value.Key}");
        try
        {
            var currentMessage = args.Messages[^1];
            Logger.LogInformation("请求OpenAI：{friendMessage}", currentMessage);

            var requestContent = new AiRequestContent(args.Messages.Select(m => new OpenAiMessage(m.Role, m.Message)).ToList());

            try
            {
                var flurlResult = await url.PostAsync(JsonContent.Create(requestContent));
                var result = await flurlResult.GetJsonAsync<AiResponse>();

                var message = result.Choices.FirstOrDefault()?.Message?.Content ?? "啊对对对";

                var user = await UserRepository.FirstOrDefaultAsync(t => t.QQ == args.SenderId);
                if (user is not null)
                {
                    await UserHistoryRepository.InsertAsync(new UserHistory
                    {
                        DateTime = DateTime.Now,
                        Message = message,
                        Role = "assistant",
                        UserId = user.Id
                    }, true);
                }

                lock (_lock)
                {
                    if (args.Type == MessageReceivers.Friend)
                    {
                        MessageManager.SendFriendMessageAsync(args.SenderId, new PlainMessage(message)).Wait();
                        Logger.LogInformation("成功回复QQ好友【{friendName}】: {message}", args.SenderName, message);
                    }
                    if (args.Type == MessageReceivers.Group)
                    {
                        var messageChain = new MessageChainBuilder()
                            .At(args.SenderId)
                            .Plain(message)
                            .Build();

                        MessageManager.SendGroupMessageAsync(args.GroupId, messageChain).Wait();
                        Logger.LogInformation("成功回复群聊【{groupName}】：{message}", args.GroupName, message);
                    }
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"""
                    --------------------
                    【{args.SenderName}({args.SenderId})】消息发送失败：
                    消息内容：{args.Messages.Last()}
                    错误类型：OpenAI请求失败
                    错误消息：{ex.Message}
                    --------------------
                    """;
                await MessageManager.SendFriendMessageAsync(BotOptions.Value.AdminQQ, new PlainMessage(errorMessage));
                Logger.LogError(errorMessage);
            }
        }
        catch (Exception ex)
        {
            var errorMessage = $"""
                --------------------
                【{args.SenderName}({args.SenderId})】消息发送失败：
                消息内容：{args.Messages.Last()}
                错误类型：无发送权限
                错误消息：{ex.Message}
                --------------------
                """;
            await MessageManager.SendFriendMessageAsync(BotOptions.Value.AdminQQ, new PlainMessage(errorMessage));
            Logger.LogError(errorMessage);
        }
    }
}