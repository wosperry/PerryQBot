using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PerryQBot.Options;
using Volo.Abp.DependencyInjection;

namespace PerryQBot.Commands.Handlers
{
    [Command("天气")]
    [Command("weather")]
    [ExposeServices(typeof(ICommandHandler))]
    public class WeatherCommandHandler : CommandHandlerBase
    {
        public IOptions<WeatherOptions> WeatherOptions { get; set; }

        public override async Task ExecuteAsync(CommandContext context)
        {
            if (string.IsNullOrEmpty(WeatherOptions.Value.QueryUrl))
            {
                ResponseMessage = "没有配置天气查询Url，无法使用此功能。";
            }

            var city = context.Message.Trim();
            var url = new Url(WeatherOptions.Value.QueryUrl)
                .SetQueryParam("key", WeatherOptions.Value.Key)
                .SetQueryParam("city", city)
                .SetQueryParam("extensions", "all");

            var result = await url.GetJsonAsync();

            if (result.status == "1")
            {
                var weatherDetail = string.Join("", (result.forecasts.casts as dynamic[]).Select(d => $"""

                 {d.date},{d.dayweather}-{d.nightweather},{d.daytemp}℃-{d.nighttemp}℃
                 """));

                ResponseMessage = $"""
                好的，这是{result.forecasts.city}的天气预报：
                {weatherDetail}
                """;
            }
        }
    }
}