using System;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;
using Mirai.Net.Utils.Scaffolds;
using PerryQBot.Commands;
using PerryQBot.Options;
using Volo.Abp.DependencyInjection;

namespace PerryQBot.CommandHandlers;

[Command("随机头像")]
[ExposeServices(typeof(ICommandHandler))]
public class AvatarCommandHandler : CommandHandlerBase
{
    public IOptions<Apis> Apis { get; set; }
    public string ImgUrl { get; set; }

    public override async Task ExecuteAsync(CommandContext context)
    {
        var rand = new Random(DateTime.Now.Millisecond).NextDouble() < 0.5 ? 0 : 1;

        var url = new Url($"{Apis.Value.UomgAPI}/rand.avatar")
            .SetQueryParam("format", "json")
            .SetQueryParam("sort", new string[] { "动漫男", "动漫女" }[rand]);

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
            builder.ImageFromUrl(ImgUrl);
        }
        return builder;
    }
}