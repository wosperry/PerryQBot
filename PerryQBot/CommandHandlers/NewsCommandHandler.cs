﻿using HtmlAgilityPack;
using Mirai.Net.Utils.Scaffolds;
using PerryQBot.Commands;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

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
            NewsSet = await NewsCache.GetOrAddAsync(CacheKey, GetFromInfoQWebsite);
            if (NewsSet.Any())
                await ClearHistoryCommandHandler.ExecuteAsync(context);
        }

        public override MessageChainBuilder OnMessageChainBuilding(MessageChainBuilder builder)
        {
            if (NewsSet.Count > 0)
            {
                var str = string.Join("", NewsSet.Select(x => $"""
                {x.Title}
                ----
                """));
                IsContinueAfterHandled = true;
                RequestMessage = "这是一段新闻，我需要你帮我翻译并整理出适合网页首页展示一列的标题，要求简短但包含重要信息" + str;
            }
            ResponseMessage = "查询失败";

            return base.OnMessageChainBuilding(builder);
        }

        /// <summary>
        /// 此方法CV了一份到查看那里，哈哈继续留坑
        /// </summary>
        /// <returns></returns>
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