using Flurl;
using Flurl.Http;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Volo.Abp.DependencyInjection;

namespace PerryQBot.Commands.Handlers
{
    [Command("天气")]
    [Command("weather")]
    [ExposeServices(typeof(ICommandHandler))]
    public class WeatherCommandHandler : CommandHandlerBase
    {
        public ClearHistoryCommandHandler ClearHistoryCommandHandler { get; set; }

        public override async Task ExecuteAsync(CommandContext context)
        {
            var url = new Url("https://weather.cma.cn/api/autocomplete")
                .SetQueryParam("q", context.Message.Trim());

            var autocomplete = await url.GetJsonAsync();
            if (autocomplete.code != 0)
            {
                ResponseMessage = "未找到城市";
                return;
            }

            var cityStr = (autocomplete.data as List<dynamic>)[0];
            var cityId = cityStr.Split('|')?[0];
            var cityName = (cityStr.Split('|')?[1].Split('|')[0] as string).Trim();
            if (string.IsNullOrEmpty(cityId))
            {
                ResponseMessage = "未找到城市";
                return;
            }

            var web = new HtmlWeb();
            var doc = web.Load($"https://weather.cma.cn/web/weather/{cityId}.html");

            // 获取一周天气数据
            var weekNodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'pull-left day')]");
            var weekData = weekNodes.Select(item =>
            {
                dynamic dynamicObject = new System.Dynamic.ExpandoObject();
                dynamicObject.Day = item.SelectSingleNode(".//div[contains(@class, 'day-item')][1]").InnerText.Trim();
                dynamicObject.Date = item.SelectSingleNode(".//div[contains(@class, 'day-item')][1]/br").NextSibling.InnerText.Trim();
                dynamicObject.DayIcon = item.SelectSingleNode(".//div[contains(@class, 'dayicon')]/img").GetAttributeValue("src", "");
                dynamicObject.DayWeather = item.SelectSingleNode(".//div[contains(@class, 'day-item')][3]").InnerText.Trim();
                dynamicObject.DayWindDirection = item.SelectSingleNode(".//div[contains(@class, 'day-item')][4]").InnerText.Trim();
                dynamicObject.DayWindPower = item.SelectSingleNode(".//div[contains(@class, 'day-item')][5]").InnerText.Trim();
                dynamicObject.HighTemperature = item.SelectSingleNode(".//div[contains(@class, 'high')]").InnerText.Trim();
                dynamicObject.LowTemperature = item.SelectSingleNode(".//div[contains(@class, 'low')]").InnerText.Trim();
                dynamicObject.NightIcon = item.SelectSingleNode(".//div[contains(@class, 'nighticon')]/img").GetAttributeValue("src", "");
                dynamicObject.NightWeather = item.SelectSingleNode(".//div[contains(@class, 'day-item')][8]").InnerText.Trim();
                dynamicObject.NightWindDirection = item.SelectSingleNode(".//div[contains(@class, 'day-item')][9]").InnerText.Trim();
                dynamicObject.NightWindPower = item.SelectSingleNode(".//div[contains(@class, 'day-item')][10]").InnerText.Trim();
                return dynamicObject;
            }).ToList();

            var dayNodes = doc.DocumentNode.SelectSingleNode("//table[@class='hour-table' and @id='hourTable_0']");
            var dayData = new List<dynamic>();
            if (dayNodes != null)
            {
                var hourNodes = dayNodes.SelectNodes(".//tr[position() > 2]");
                if (hourNodes != null)
                {
                    foreach (var hourNode in hourNodes)
                    {
                        dynamic hourData = new System.Dynamic.ExpandoObject();
                        hourData.Time = hourNode.SelectSingleNode("./td[1]").InnerText.Trim();
                        hourData.Temperature = hourNode.SelectSingleNode("./td[2]").InnerText.Trim();
                        hourData.Weather = hourNode.SelectSingleNode("./td[3]").InnerText.Trim();
                        hourData.Rain = hourNode.SelectSingleNode("./td[4]").InnerText.Trim();
                        hourData.WindSpeed = hourNode.SelectSingleNode("./td[5]").InnerText.Trim();
                        hourData.WindDirection = hourNode.SelectSingleNode("./td[6]").InnerText.Trim();
                        hourData.Pressure = hourNode.SelectSingleNode("./td[7]").InnerText.Trim();
                        hourData.Humidity = hourNode.SelectSingleNode("./td[8]").InnerText.Trim();
                        hourData.Cloud = hourNode.SelectSingleNode("./td[9]").InnerText.Trim();
                        dayData.Add(hourData);
                    }
                }
            }

            // 组装最终的动态对象
            dynamic result = new System.Dynamic.ExpandoObject();
            result.WeekData = weekData;
            result.DayData = dayData;

            var str = JsonConvert.SerializeObject(result);

            await ClearHistoryCommandHandler.ExecuteAsync(context);

            IsContinueAfterHandled = true;
            RequestMessage = $"""
                {str}
                ---------
                分析{cityName}的天气数据，不允许胡编乱造假数据。记住一定要加上数据来源于中国气象局网站weather.cma.cn
                此回复用于群聊，尽可能不要换行两次，而且不应用markdown格式。参考下列格式输出，你仅被允许在方括号处自由添加分析和自由发挥，比如【分析】下面是格式：

                {cityName}市今天天气：晴，气温：-5℃~+5℃，风力：3级，风向：东北风
                --------------------------------
                1月1日 星期一 晴 -5℃~+5℃ 东北风3级
                1月2日 星期二 晴 -5℃~+5℃ 东北风3级
                ...
                1月7日 星期日 晴 -5℃~+5℃ 东北风3级
                --------------------------------
                以上数据来源于中国气象局网站weather.cma.cn

                根据气象数据显示，今天{cityName}市【详细分析今天数据，包括云雨风日照等】（例如：天气xxx，适合xxx,紫外线强度xxx，建议xxx，注意xxx）。
                """;
        }
    }
}