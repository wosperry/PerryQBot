using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;
using Mirai.Net.Utils.Scaffolds;
using PerryQBot.Options;

namespace PerryQBot.Commands.Handlers
{
    [Command("二刺螈")]
    [Command("二刺猿")]
    [Command("二次元")]
    public class ScgCommandHandler : CommandHandlerBase
    {
        public IOptions<RandomImageOptions> RandomImageOptions { get; set; }
        public string ImgUrl { get; set; }

        public override async Task ExecuteAsync(CommandContext context)
        {
            var url = new Url(RandomImageOptions.Value.Url)
                .SetQueryParam("format", "json")
                .SetQueryParam("sort", "svg");

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
}