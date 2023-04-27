using Microsoft.EntityFrameworkCore;
using PerryQBot.Commands;
using PerryQBot.EntityFrameworkCore.Entities;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace PerryQBot.CommandHandlers;

[Command("preset")]
[Command("预设")]
[ExposeServices(typeof(ICommandHandler))]
public class PresetSetCommandHandler : CommandHandlerBase
{
    public IRepository<User> UserRepository { get; set; }

    public override async Task ExecuteAsync(CommandContext context)
    {
        var (isCommand, commandString, messageString) = this.TryGetCommand(context.Message);
        var user = await (await UserRepository.WithDetailsAsync(x => x.History)).FirstOrDefaultAsync(t => t.QQ == context.SenderId);

        if (user is null)
        {
            await UserRepository.InsertAsync(new User
            {
                QQ = context.SenderId,
                QQNickName = context.SenderName,
                History = new List<UserHistory>(),
                Preset = messageString
            }, true);
        }
        else
        {
            user.Preset = messageString;
            user.History.Clear();
            await UserRepository.UpdateAsync(user, true);
        }

        ResponseMessage = $"预设修改成功";
    }
}