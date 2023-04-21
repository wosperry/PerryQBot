using Microsoft.EntityFrameworkCore;
using PerryQBot.EntityFrameworkCore.Entities;
using Volo.Abp.Domain.Repositories;

namespace PerryQBot.Commands.Handlers
{
    [Command("预设", "修改我的预设")]
    public class PresetSetCommandHandler : CommandHandlerBase
    {
        public IRepository<User> UserRepository { get; set; }

        public override async Task<string> HandleAndResponseAsync(CommandContext context)
        {
            var presetMessage = this.GetMessageString(context.Message);
            var user = await UserRepository.FirstOrDefaultAsync(t => t.QQ == context.SenderId);

            if (user is null)
            {
                await UserRepository.InsertAsync(new User
                {
                    QQ = context.SenderId,
                    QQNickName = context.SenderName,
                    History = new List<UserHistory>(),
                    Preset = presetMessage
                });
            }
            else
            {
                user.Preset = presetMessage;
                user.History.Clear();
                await UserRepository.UpdateAsync(user);
            }

            return $"您的预设成功修改为：{presetMessage}";
        }
    }
}