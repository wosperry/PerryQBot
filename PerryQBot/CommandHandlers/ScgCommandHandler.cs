using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;
using Mirai.Net.Utils.Scaffolds;
using PerryQBot.Commands;
using PerryQBot.Options;
using Volo.Abp.DependencyInjection;

namespace PerryQBot.CommandHandlers;

[Command("二刺螈")]
[Command("二刺猿")]
[Command("二次元")]
[ExposeServices(typeof(ICommandHandler))]
public class ScgCommandHandler : CommandHandlerBase
{
    public IOptions<Apis> Apis { get; set; }
    public string ImgUrl { get; set; }

    public override async Task ExecuteAsync(CommandContext context)
    {
        var rand = new Random(DateTime.Now.Millisecond).Next(1, 2); // 1横版 2竖版

        var url = new Url($"{Apis.Value.UomgAPI}/rand.img{rand}")
            .SetQueryParam("format", "json")
            .SetQueryParam("sort", "二次元");

        // 偷懒，直接dynamic了
        var result = await url.GetJsonAsync();
        if (result.code == 1)
        {
            ImgUrl = result.imgurl;
        }
        else
        {
            ResponseMessage = "获取图片失败";
        }
    }

    public override MessageChainBuilder OnMessageChainBuilding(MessageChainBuilder builder)
    {
        if (!string.IsNullOrEmpty(ImgUrl))
        {
            // 前面有个At消息，清理掉
            builder.Clear();
            builder.ImageFromUrl(ImgUrl);
        }
        return builder;
    }
}