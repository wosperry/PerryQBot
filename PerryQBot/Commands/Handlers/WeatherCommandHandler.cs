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

            IsContinueAfterHandled = true;
            RequestMessage = $"""
                城市是接口返回中的location
                {now}
                ---------
                加上你对天气的分析，不允许胡编乱造假数据。记住一定要加上数据来源于中国气象局网站weather.cma.cn
                此回复用于群聊，尽可能不要换行两次，而且不应用markdown格式
                """;
        }
    }
}