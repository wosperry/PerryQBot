﻿using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;
using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Concretes;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Utils.Scaffolds;
using Newtonsoft.Json;
using PerryQBot.Options;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DistributedLocking;

public class OpenAIRequestingBackgroundJob : BackgroundJob<OpenAIRequestingBackgroundJobArgs>, ITransientDependency
{
    public IOptions<OpenAiOptions> OpenAiOptions { get; set; }
    public IOptions<MiraiBotOptions> BotOptions { get; set; }
    public IAbpDistributedLock DistributedLock { get; set; }

    // TODO: 考虑发消息改到一个独立的后台任务中
    public override async void Execute(OpenAIRequestingBackgroundJobArgs args)
    {
        var url = new Url(new Uri(OpenAiOptions.Value.CompletionUrl))
            .WithHeader("Authorization", $"Bearer {OpenAiOptions.Value.Key}");
        try
        {
            var currentMessage = args.Messages[^1];
            Logger.LogInformation("请求OpenAI：{friendMessage}", currentMessage);

            var requestContent = new AiRequestContent(args.Messages.Select(m => new OpenAiMessage("user", m)).ToList());

            var flurlResult = await url.PostAsync(JsonContent.Create(requestContent));
            var result = await flurlResult.GetJsonAsync<AiResponse>();
            var message = " " + (result.Choices.FirstOrDefault()?.Message?.Content?.Replace("\r\n", "\r").Replace("\r\r", "\r") ?? "啊对对对");

            lock ("bot")
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
            Logger.LogError("OpenAI请求失败", ex);
            await MessageManager.SendFriendMessageAsync(BotOptions.Value.AdminQQ, new PlainMessage(JsonConvert.SerializeObject(new
            {
                ErrorType = "OpenAI请求失败",
                ErrorMessage = "执行失败：" + ex.Message,
                args.SenderId,
                args.SenderName,
                Message = args.Messages.Last(),
            }, Formatting.Indented)));
        }
    }
}