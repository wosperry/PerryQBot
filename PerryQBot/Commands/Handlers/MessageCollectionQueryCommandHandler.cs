using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PerryQBot.EntityFrameworkCore.Entities;
using PerryQBot.Options;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace PerryQBot.Commands.Handlers;

[Command("查询收藏")]
[ExposeServices(typeof(ICommandHandler))]
public class MessageCollectionQueryCommandHandler : CommandHandlerBase
{
    public IRepository<DialogCollection> DialogCollectionRepository { get; set; }
    public IOptions<MessageCollectionOptions> MessageCollectionOptions { get; set; }

    public override async Task ExecuteAsync(CommandContext context)
    {
        var result = await (await DialogCollectionRepository.GetQueryableAsync())
            .Where(t => t.Message.Contains(context.Message, StringComparison.OrdinalIgnoreCase)
                || t.QuoteMessage.Contains(context.Message, StringComparison.OrdinalIgnoreCase))
            .Take(MessageCollectionOptions.Value.MaxResultCount)
            .ToListAsync();

        ResponseMessage = string.Join("------------", result.Select(x =>
        {
            return $"""

            收藏者: {x.UserName}({x.UserQQ})
            收藏备注：{x.Message}
            收藏时间：{x.DateTime:yyyy-MM-dd HH:mm:ss}
            收藏内容：{x.QuoteMessage}

            """;
        }));
    }
}