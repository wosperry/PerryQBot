﻿using Microsoft.EntityFrameworkCore;
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
            .Where(t => t.Message.Contains(context.Message)
                || t.QuoteMessage.Contains(context.Message))
            .Take(MessageCollectionOptions.Value.MaxResultCount)
            .ToListAsync();

        ResponseMessage = "OK，这是您要的结果：\r\n" + string.Join("-------------------", result.Select(x =>
        {
            return $"""

            {x.UserName}({x.UserQQ}) 收藏于 [{x.DateTime:yyyy-MM-dd HH:mm:ss}]
            内容：{x.Message}
            引用：{x.QuoteMessage}

            """;
        }));
    }
}