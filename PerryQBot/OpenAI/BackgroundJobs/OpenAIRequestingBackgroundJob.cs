using Flurl;
using Flurl.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Concretes;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Utils.Scaffolds;
using Newtonsoft.Json;
using PerryQBot.EntityFrameworkCore.Entities;
using PerryQBot.Options;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace PerryQBot.OpenAI.BackgroundJobs;

public class OpenAIRequestingBackgroundJob : BackgroundJob<OpenAIRequestingBackgroundJobArgs>, ITransientDependency
{
    public IOptions<OpenAiOptions> OpenAiOptions { get; set; }
    public IOptions<MiraiBotOptions> BotOptions { get; set; }
    public IAbpDistributedLock DistributedLock { get; set; }
    public IUnitOfWorkManager UnitOfWorkManager { get; set; }

    private static readonly object _lock = new();

    // TODO: 考虑发消息改到一个独立的后台任务中

    public override async void Execute(OpenAIRequestingBackgroundJobArgs args)
    {
        var url = new Url(new Uri(OpenAiOptions.Value.CompletionUrl))
            .WithHeader("Authorization", $"Bearer {OpenAiOptions.Value.Key}");
        try
        {
            var m = JsonConvert.SerializeObject(args.Messages);
            Logger.LogInformation("请求OpenAI：{m}", m);

            var requestContent = new AiRequestContent(args.Messages.Select(m => new OpenAiMessage(m.Role, m.Content)).ToList());

            try
            {
                var flurlResult = await url.PostAsync(JsonContent.Create(requestContent));
                var result = await flurlResult.GetJsonAsync<AiResponse>();

                var message = result.Choices.FirstOrDefault()?.Message?.Content ?? "啊对对对";

                await AddHistoryAsync(args.SenderId, message);

                lock (_lock)
                {
                    var messageChainBuilder = new MessageChainBuilder();

                    if (message.Contains("(=^･^=)"))
                    {
                        message = message.Replace("(=^･^=)", "");
                        messageChainBuilder.ImageFromUrl("https://wosperry.com/lsky/img/2023/04/26/64480d93a17c9.png");
                    }
                    if (message.Contains("咕噜"))
                    {
                        message = message.Replace("咕噜", "");
                        messageChainBuilder.ImageFromUrl("https://wosperry.com/lsky/img/2023/04/26/64481b95c56e7.png");
                    }

                    if (args.Type == MessageReceivers.Friend)
                    {
                        messageChainBuilder.Plain(message);
                        MessageManager.SendFriendMessageAsync(args.SenderId, messageChainBuilder.Build()).Wait();
                        Logger.LogInformation("成功回复QQ好友【{friendName}】: {message}", args.SenderName, message);
                    }
                    if (args.Type == MessageReceivers.Group)
                    {
                        if (!string.IsNullOrEmpty(args.SenderId))
                        {
                            messageChainBuilder.At(args.SenderId).Plain(" ");
                        }

                        messageChainBuilder.Plain(message);

                        MessageManager.SendGroupMessageAsync(args.GroupId, messageChainBuilder.Build()).Wait();
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

    public async Task AddHistoryAsync(string qq, string message)
    {
        using var uow = UnitOfWorkManager.Begin();
        var userRepository = uow.ServiceProvider.GetService<IRepository<User>>();
        var user = await (await userRepository.WithDetailsAsync(x => x.History))
            .FirstOrDefaultAsync(t => t.QQ == qq);

        user?.History.Add(new()
        {
            DateTime = DateTime.Now,
            Message = message,
            Role = "assistant",
            UserId = user.Id
        });
        await uow.CompleteAsync();
    }
}