﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Mirai.Net.Sessions.Http.Managers;
using Quartz;
using Volo.Abp.BackgroundWorkers.Quartz;

namespace PerryQBot.QQBot
{
    public class GroupNewsBackgroundWorker : QuartzBackgroundWorkerBase
    {
        public GroupNewsBackgroundWorker()
        {
            Trigger = TriggerBuilder.Create().WithIdentity(nameof(GroupNewsBackgroundWorker))
                .WithCronSchedule("0 34 17 ? * *")
                .StartNow().Build();
        }

        public override async Task Execute(IJobExecutionContext context)
        {
            await MessageManager.SendFriendMessageAsync("593281239", $"{DateTime.Now:HH:mm:ss}");
        }
    }
}