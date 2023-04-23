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

            var strResult = await url.GetStringAsync();
            var result = JsonConvert.DeserializeObject<GaodeWeatherResponse>(strResult);

            if (result.status == GaodeResponseResultStatus.成功)
            {
                var weatherDetail = string.Join("", result.forecasts.First().casts.Select(d => $"""
                {d.date},{d.dayweather}-{d.nightweather},{d.daytemp}℃-{d.nighttemp}℃

                """));

                var weatherMessage = $"""
                {result.forecasts.First().city}的天气预报：
                {weatherDetail}
                """;

                if (WeatherOptions.Value.ResponseByAi)
                {
                    // Handle 之后继续后面的AI请求
                    IsContinueAfterHandled = true;
                    // 修改请求内容为新的
                    RequestMessage = $"""
                    我要求你直接输出以下天气信息的完整内容，并最后在其后面附加上你的建议。

                    {weatherMessage}
                    """;
                }
                else
                {
                    ResponseMessage = weatherMessage;
                }
            }
        }
    }
}

public class GaodeWeatherResponse
{
    public GaodeResponseResultStatus status { get; set; }
    public string info { get; set; }
    public string infocode { get; set; }
    public List<GaodeWeatherResponseForecasts> forecasts { get; set; }
}

public class GaodeWeatherResponseForecasts
{
    public string city { get; set; }
    public string adcode { get; set; }
    public string province { get; set; }
    public string reporttime { get; set; }
    public List<GaodeWeatherResponseForecastsCasts> casts { get; set; }
}

public class GaodeWeatherResponseForecastsCasts
{
    public string date { get; set; }
    public GaodeWeek week { get; set; }
    public string dayweather { get; set; }
    public string nightweather { get; set; }
    public string daytemp { get; set; }
    public string nighttemp { get; set; }
    public string daywind { get; set; }
    public string nightwind { get; set; }
}

public enum GaodeWeek
{
    周一 = 1,
    周二 = 2,
    周三 = 3,
    周四 = 4,
    周五 = 5,
    周六 = 6,
    周日 = 7
}

public enum GaodeResponseResultStatus
{
    失败 = 0,
    成功 = 1
}