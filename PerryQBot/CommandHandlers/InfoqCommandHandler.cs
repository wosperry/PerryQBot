//using System.Text;
//using Flurl;
//using HtmlAgilityPack;
//using PerryQBot.Commands;
//using Volo.Abp.DependencyInjection;

//namespace PerryQBot.CommandHandlers
//{
//    [Command("InfoQ")]
//    [ExposeServices(typeof(ICommandHandler))]
//    public class InfoqCommandHandler : CommandHandlerBase
//    {
//        public override Task ExecuteAsync(CommandContext context)
//        {
//            // 创建HtmlWeb实例并加载URL
//            HtmlWeb web = new HtmlWeb();
//            HtmlDocument doc = web.Load("https://www.infoq.com/news/");

//            // 获取所有li元素
//            HtmlNodeCollection liNodes = doc.DocumentNode.SelectNodes("//*[@id='infoq']/main/section/div/div/div[2]/div/ul/li");

//            // 创建News列表
//            List<News> newsList = new List<News>();

//            // 循环遍历所有li元素并将其添加到News列表中
//            foreach (HtmlNode liNode in liNodes)
//            {
//                News news = new News();

//                // 获取li元素下的标题、作者、摘要和日期元素
//                HtmlNode titleNode = liNode.SelectSingleNode("./div/div/h3/a");
//                HtmlNode authorNode = liNode.SelectSingleNode("./div/div/div[1]/span/a");
//                HtmlNode summaryNode = liNode.SelectSingleNode("./div/div/div[2]/div/div/span/a");
//                HtmlNode dateNode = liNode.SelectSingleNode("./div/div/div[2]/div/span/span");

//                // 将元素的值设置为News对象的属性
//                news.Title = titleNode.InnerText;
//                news.Author = authorNode.InnerText;
//                news.Summary = summaryNode.InnerText;
//                news.Date = dateNode.InnerText;

//                // 将News对象添加到列表中
//                newsList.Add(news);
//            }

//            // 打印News列表中的所有News对象
//            foreach (News news in newsList)
//            {
//                Console.WriteLine("Title: " + news.Title);
//                Console.WriteLine("Author: " + news.Author);
//                Console.WriteLine("Summary: " + news.Summary);
//                Console.WriteLine("Date: " + news.Date);
//                Console.WriteLine("----------------------");
//            }

//            Console.WriteLine();
//        }
//    }
//}

//public class News
//{
//    public string Title { get; set; }
//    public string Author { get; set; }
//    public string Summary { get; set; }
//    public string Date { get; set; }
//}