using HtmlAgilityPack;
using Mirai.Net.Utils.Scaffolds;
using PerryQBot.Commands;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Websocket.Client;

namespace PerryQBot.CommandHandlers
{
    [Command("news")]
    [Command("新闻")]
    [ExposeServices(typeof(ICommandHandler))]
    public class NewsCommandHandler : CommandHandlerBase
    {
        private const string CacheKey = "news_infoq";
        public ClearHistoryCommandHandler ClearHistoryCommandHandler { get; set; }
        public IDistributedCache<List<SimpleNews>> NewsCache { get; set; }
        public List<SimpleNews> NewsSet { get; set; }

        public override async Task ExecuteAsync(CommandContext context)
        {
            NewsSet = await NewsCache.GetOrAddAsync(CacheKey, GetFromInfoQWebsite,
                () => new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) });
            if (NewsSet.Any())
                await ClearHistoryCommandHandler.ExecuteAsync(context);
        }

        public override MessageChainBuilder OnMessageChainBuilding(MessageChainBuilder builder)
        {
            if (NewsSet.Count > 0)
            {
                var str = string.Join("", NewsSet.Select(x => $"""
                {x.Id}. {x.Title}
                ----
                """));
                IsContinueAfterHandled = true;
                AutoResponse = false;
                RequestMessage = "这是一段新闻，希望Mochi用猫猫的语气帮我翻译并润色，要求输出所有的新闻标题不能缺少一条，要记得换行哦，当然因为是要给群里回复用的，所以结果要尽量紧凑不能出现换行两次哦。然后你应该是一个讲述新闻的猫猫，所以输出的时候不要表现出你在“翻译”，你输出的时候记得在最开始说大家好，现在是猫猫新闻时间，Mochi来给大家讲新闻啦。" + str;
            }
            else
            {
                ResponseMessage = "查询失败";
            }
            return new MessageChainBuilder();
        }

        /// <summary>
        /// 此方法CV了一份到查看那里，哈哈继续留坑
        /// </summary>
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
}

public class SimpleNews
{
    public int Id { get; set; }
    public string Topic { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Author { get; set; }
}