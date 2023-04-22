using Microsoft.EntityFrameworkCore;
using PerryQBot.EntityFrameworkCore.Entities;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace PerryQBot.Commands.Handlers;

[Command("清空")]
[Command("清空记录")]
[Command("清空記錄")]
[Command("清除记录")]
[Command("清除記錄")]
[ExposeServices(typeof(ICommandHandler))]
public class ClearHistoryCommandHandler : CommandHandlerBase
{
    public IRepository<User> UserRepository { get; set; }

    public override async Task<string> HandleAndResponseAsync(CommandContext context)
    {
        var user = await (await UserRepository.WithDetailsAsync(x => x.History)).FirstOrDefaultAsync(t => t.QQ == context.SenderId);

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

        return "您的历史已清空";
    }
}