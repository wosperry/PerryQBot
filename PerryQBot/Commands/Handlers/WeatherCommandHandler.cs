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

            if (result.Status == GaodeResponseResultStatus.成功)
            {
                var weatherDetail = string.Join("", result.Forecasts.Casts.Select(d => $"""
                {d.Date},{d.DayWeather}-{d.NightWeather},{d.DayTemp}℃-{d.NightTemp}℃

                """));

                ResponseMessage = $"""
                {result.Forecasts.City}的天气预报：
                {weatherDetail}
                """;
            }
        }
    }
}

public class GaodeWeatherResponse
{
    public GaodeResponseResultStatus Status { get; set; }
    public string Info { get; set; }
    public string Infocode { get; set; }
    public GaodeWeatherResponseForecasts Forecasts { get; set; }
}

public class GaodeWeatherResponseForecasts
{
    public string City { get; set; }
    public string AdCode { get; set; }
    public string Province { get; set; }
    public string ReportTime { get; set; }
    public List<GaodeWeatherResponseForecastsCasts> Casts { get; set; }
}

public class GaodeWeatherResponseForecastsCasts
{
    public string Date { get; set; }
    public GaodeWeek Week { get; set; }
    public string DayWeather { get; set; }
    public string NightWeather { get; set; }
    public string DayTemp { get; set; }
    public string NightTemp { get; set; }
    public string DayWind { get; set; }
    public string NightWind { get; set; }
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