﻿using PerryQBot.EntityFrameworkCore.Entities;
using Volo.Abp.Domain.Repositories;

namespace PerryQBot.Commands.Handlers;

[Command("查看预设", "查看我的预设")]
public class PresetGetCommandHandler : CommandHandlerBase
{
    public IRepository<User> UserRepository { get; set; }

    public override async Task<string> HandleAndResponseAsync(CommandContext context)
    {
        var user = await UserRepository.FirstOrDefaultAsync(t => t.QQ == context.SenderId);
        return $"您的预设为：{user?.Preset}";
    }
}