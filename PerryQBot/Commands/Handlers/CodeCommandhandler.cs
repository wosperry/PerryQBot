﻿namespace PerryQBot.Commands.Handlers;

[Command("代码")]
public class CodeCommandhandler : CommandHandlerBase
{
    public override async Task<string> HandleAndResponseAsync(CommandContext context)
    {
        await Task.CompletedTask;
        return "https://github.com/wosperry/PerryQBot";
    }
}