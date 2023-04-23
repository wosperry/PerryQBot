using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PerryQBot.Options;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Auditing;

namespace PerryQBot.Commands.Handlers
{
    [Command("天气")]
    [Command("weather")]
    [ExposeServices(typeof(ICommandHandler))]
    public class WeatherCommandHandler : CommandHandlerBase
    {
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

            var cityId = (autocomplete.data as List<dynamic>)[0].Split('|')?[0];
            if (string.IsNullOrEmpty(cityId))
            {
                ResponseMessage = "未找到城市";
                return;
            }

            var nowUrl = new Url($"https://weather.cma.cn/api/now/{cityId}");
            var now = await nowUrl.GetStringAsync();

            var climateUrl = new Url($"https://weather.cma.cn/api/climate?stationid={cityId}");
            var climate = await climateUrl.GetStringAsync();

            IsContinueAfterHandled = true;
            RequestMessage = $"""
                城市是接口返回中的location
                {now}
                {climate}
                ---------
                分析天气数据，按日期分成多行，显示今天开始的三天数据详情，结束的地方强调数据来源于中国气象局网站weather.cma.cn。然后加上你对天气的分析，比如：今天天气晴朗，气温适宜，空气质量优，适合户外活动。分析和展示要分开，此回复用于群聊，不应用markdown格式。
                """;
        }
    }
}