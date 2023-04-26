using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using Mirai.Net.Data.Messages;
using PerryQBot.Options;
using Quartz;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BackgroundWorkers.Quartz;

namespace PerryQBot.QQBot;

public class GroupNewsBackgroundWorker : QuartzBackgroundWorkerBase
{
    private const string CacheKey = "news_infoq";

    public GroupNewsBackgroundWorker()
    {
        Trigger = TriggerBuilder.Create().WithIdentity(nameof(GroupNewsBackgroundWorker))
            .WithCronSchedule("0 30 9 ? * *")
            //.WithSimpleSchedule(x => x.WithIntervalInMinutes(2))
            .StartNow().Build();
        JobDetail = JobBuilder.Create<GroupNewsBackgroundWorker>().WithIdentity(nameof(GroupNewsBackgroundWorker)).Build();
    }

    public override async Task Execute(IJobExecutionContext context)
    {
        var newsOptions = ServiceProvider.GetService<IOptions<NewsOptions>>();
        var jobManager = ServiceProvider.GetService<IBackgroundJobManager>();

        var news = await GetFromInfoQWebsite();
        var str = string.Join("", news.Select(x => $"""
                {x.Id}. {x.Title}
                ----
                """));

        var requestString = "这是一段新闻，希望Mochi用猫猫的语气帮我翻译并润色，要求输出所有的新闻标题不能缺少一条，要记得换行哦，当然因为是要给群里回复用的，所以结果要尽量紧凑不能出现换行两次哦。然后你应该是一个讲述新闻的猫猫，所以输出的时候不要表现出你在“翻译”，你输出的时候记得在最开始说大家好，现在是猫猫新闻时间，Mochi来给大家讲新闻啦。" + str;

        foreach (var group in newsOptions.Value.Groups)
        {
            await jobManager.EnqueueAsync(new OpenAIRequestingBackgroundJobArgs

            {
                GroupId = group,
                GroupName = group, // 配置没有群名
                Type = MessageReceivers.Group,
                Messages = new List<OpenAiMessage>
                {
                     new OpenAiMessage("user", requestString)
                }
            });
        }
    }

    private Task<List<SimpleNews>> GetFromInfoQWebsite()
    {
        // 创建HtmlWeb对象，用于获取网页
        var web = new HtmlWeb();
        var doc = web.Load("https://www.infoq.com/news/");

        var newsNodes = doc.DocumentNode.SelectNodes("//*[@data-tax='news']/li");
        var list = newsNodes.Select((node, index) => new SimpleNews
        {
            Id = index + 1,
            Topic = node.SelectNodes(".//div[contains(@class,'card__topics')]/span/a[1]")
                        .FirstOrDefault()?.InnerHtml?.Replace("\n", "").Trim(),
            Title = node.SelectNodes(".//h3[contains(@class,'card__title')]/a[1]")
                        .FirstOrDefault()?.InnerHtml?.Replace("\n", "").Trim(),
            Content = node.SelectNodes(".//p[contains(@class,'card__excerpt')]")
                        .FirstOrDefault()?.InnerHtml?.Replace("\n", "").Trim(),
            Author = node.SelectNodes(".//div[contains(@class,'card__authors')]/span/a[1]")
                        .FirstOrDefault()?.InnerHtml?.Replace("\n", "").Trim()
        }).ToList();

        return Task.FromResult(list);
    }
}