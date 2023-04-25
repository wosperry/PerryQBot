using HtmlAgilityPack;
using Mirai.Net.Utils.Scaffolds;
using PerryQBot.Commands;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace PerryQBot.CommandHandlers
{
    [Command("查看新闻")]
    [Command("viewnews")]
    [ExposeServices(typeof(ICommandHandler))]
    public class ViewNewsCommandHandler : CommandHandlerBase
    {
        private const string CacheKey = "news_infoq";
        public ClearHistoryCommandHandler ClearHistoryCommandHandler { get; set; }
        public IDistributedCache<List<SimpleNews>> NewsCache { get; set; }
        public SimpleNews News { get; set; }

        public override async Task ExecuteAsync(CommandContext context)
        {
            if (int.TryParse(context.Message, out int id))
            {
                var newsSet = await NewsCache.GetOrAddAsync(CacheKey, GetFromInfoQWebsite);
                if (newsSet.Any())
                {
                    await ClearHistoryCommandHandler.ExecuteAsync(context);
                    News = newsSet.FirstOrDefault(t => t.Id == id);
                }
            }
        }

        public override MessageChainBuilder OnMessageChainBuilding(MessageChainBuilder builder)
        {
            if (News is not null)
            {
                var str = $"""

                主题：{News.Topic}
                标题：{News.Title}
                作者：{News.Author}
                内容：{News.Content}
                """;
                AutoResponse = false;
                IsContinueAfterHandled = true;
                RequestMessage = "这是一段新闻，我需要你帮我翻译成中文，要求保持原格式输出" + str;
            }
            else
            {
                ResponseMessage = "查询失败";
            }

            return builder;
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