﻿using Mirai.Net.Data.Messages;
using Mirai.Net.Sessions.Http.Managers;
using Volo.Abp.DependencyInjection;

public class PresetCommandHandler : CommandHandlerBase, ITransientDependency
{
    public override string GetCommand() => "#预设";

    public override async Task HandleAsync(CommandContext context)
    {
        if (context.Type == MessageReceivers.Friend)
        {
            await MessageManager.SendFriendMessageAsync(context.SenderId, "这是一段预设信息");
        }
    }
}