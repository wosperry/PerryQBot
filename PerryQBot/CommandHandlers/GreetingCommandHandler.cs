﻿using PerryQBot.Commands;
using Volo.Abp.DependencyInjection;

namespace PerryQBot.CommandHandlers;

[Command("你好")]
[ExposeServices(typeof(ICommandHandler))]
public class GreetingCommandHandler : CommandHandlerBase
{
    public override async Task ExecuteAsync(CommandContext context)
    {
        await Task.CompletedTask;
        ResponseMessage = $"你好，{context.SenderName}";
    }
}