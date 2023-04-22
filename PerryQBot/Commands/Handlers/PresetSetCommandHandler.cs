﻿using Microsoft.EntityFrameworkCore;
using PerryQBot.EntityFrameworkCore.Entities;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace PerryQBot.Commands.Handlers
{
    [Command("preset")]
    [Command("预设")]
    [Command("預設")]
    [ExposeServices(typeof(ICommandHandler))]
    public class PresetSetCommandHandler : CommandHandlerBase
    {
        public IRepository<User> UserRepository { get; set; }

        public override async Task<string> HandleAndResponseAsync(CommandContext context)
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
                });
            }
            else
            {
                user.Preset = messageString;
                user.History.Clear();
                await UserRepository.UpdateAsync(user);
            }

            return $"您的预设成功修改为：{messageString}";
        }
    }
}