using Microsoft.EntityFrameworkCore;
using PerryQBot.Commands;
using PerryQBot.EntityFrameworkCore.Entities;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace PerryQBot.CommandHandlers;

[Command("清空")]
[Command("清空记录")]
[ExposeServices(typeof(ICommandHandler), typeof(ClearHistoryCommandHandler))]
public class ClearHistoryCommandHandler : CommandHandlerBase
{
    public IRepository<User> UserRepository { get; set; }

    public override async Task ExecuteAsync(CommandContext context)
    {
        var user = await (await UserRepository.WithDetailsAsync(x => x.History)).FirstOrDefaultAsync(t => t.QQ == context.SenderId);

        if (user is not null)
        {
            var history = user.History.OrderByDescending(x => x.Id).Take(BotOptions.Value.MaxHistory).ToList();
            await UserRepository.UpdateAsync(user, true);
        }
        if (user is null)
        {
            await UserRepository.InsertAsync(new User
            {
                QQ = context.SenderId,
                QQNickName = context.SenderName,
                History = new List<UserHistory>(),
                Preset = ""
            });
        }
        else
        {
            user.History.Clear();
            await UserRepository.UpdateAsync(user);
        }

        ResponseMessage = "您的历史已清空";
    }
}