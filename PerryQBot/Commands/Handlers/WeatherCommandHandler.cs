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
                分析天气数据，按日期分成多行，显示今天开始的三天数据详情。然后加上你对天气的分析。
                此回复用于群聊，尽可能不要换行两次，而且不应用markdown格式。要求参考以下格式回复：
                ---------
                某城市5月1日天气（数据来源于中国气象局网站weather.cma.cn）：
                5月1日，多云，14~20℃，微风
                5月2日，暴雨转多云，8~14℃，微风
                5月3日，晴，20~32℃，微风

                今天天气晴朗，明天有雨，后天有风，预计明天会下雨，后天会有风，但是后天的风不会很大，所以不会影响出行。
                日照时间较长，紫外线较强，注意防晒。
                """;
        }
    }
}