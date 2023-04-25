using System.Reflection;
using System.Windows.Input;
using Castle.Core.Internal;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Data.Messages;
using Mirai.Net.Sessions.Http.Managers;
using PerryQBot.Commands;
using PerryQBot.Options;
using Quartz;
using Volo.Abp.BackgroundWorkers.Quartz;
using Mirai.Net.Utils.Scaffolds;

namespace PerryQBot.QQBot
{
    public class GroupNewsBackgroundWorker : QuartzBackgroundWorkerBase
    {
        public GroupNewsBackgroundWorker()
        {
            Trigger = TriggerBuilder.Create().WithIdentity(nameof(GroupNewsBackgroundWorker))
                .WithCronSchedule("0 9 18 ? * *")
                .StartNow().Build();
            JobDetail = JobBuilder.Create<GroupNewsBackgroundWorker>().WithIdentity(nameof(GroupNewsBackgroundWorker)).Build();
        }

        public override async Task Execute(IJobExecutionContext context)
        {
            var options = ServiceProvider.GetService<IOptions<NewsOptions>>();
            var commands = ServiceProvider.GetService<IEnumerable<ICommandHandler>>();
            var botOptions = ServiceProvider.GetService<IOptions<MiraiBotOptions>>();

            foreach (var commandHandler in commands)
            {
                var (isCommand, commandString, messageString) = commandHandler.TryGetCommand("#news", botOptions.Value.CommandPrefix);
                if (isCommand)
                {
                    var c = new CommandContext
                    {
                        Type = MessageReceivers.Group,
                        Message = messageString,
                        CommandString = commandString,
                        MessageChain = new MessageChainBuilder().Build()
                    };

                    foreach (var group in options.Value.Groups)
                    {
                        c.GroupId = group;
                        commandHandler.RequestMessage = c.Message;
                        await commandHandler.HandleAsync(c);
                    }
                }
            }
        }
    }
}