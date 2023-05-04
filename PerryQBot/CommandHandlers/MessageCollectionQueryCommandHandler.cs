using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PerryQBot.Commands;
using PerryQBot.EntityFrameworkCore.Entities;
using PerryQBot.Options;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace PerryQBot.CommandHandlers;

[Command("查询收藏")]
[ExposeServices(typeof(ICommandHandler))]
public class MessageCollectionQueryCommandHandler : CommandHandlerBase
{
    public IRepository<DialogCollection> DialogCollectionRepository { get; set; }
    public IOptions<MessageCollectionOptions> MessageCollectionOptions { get; set; }

    public override async Task ExecuteAsync(CommandContext context)
    {
        var message = context.Message.Trim();
        var page = 1;
        var pageMatch = Regex.Match(message, "第([0-9]{1,3})页");
        if (pageMatch.Success)
        {
            int.TryParse(pageMatch.Groups[1].ToString(), out page);
            message = message.Replace($"第{pageMatch.Groups[1]}页", "").Trim();
        }

        var result = await (await DialogCollectionRepository.GetQueryableAsync())
            .Where(t => t.Message.Contains(message)
                || t.QuoteMessage.Contains(message))
            .OrderBy(x => x.DateTime)
            .Skip(MessageCollectionOptions.Value.MaxResultCount * (page - 1))
            .Take(MessageCollectionOptions.Value.MaxResultCount)
            .ToListAsync();

        if (result.Count > 0)
        {
            ResponseMessage = "\r\n" + string.Join("-------------------",
                result.Select(x =>
                    $"""
                    原文：{x.QuoteMessage}
                    备注：{x.Message}
                    """));
        }
        else
        {
            ResponseMessage = "没有找到您要的结果呢";
        }
    }
}