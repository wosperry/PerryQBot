using Mirai.Net.Utils.Scaffolds;
using PerryQBot.Commands;
using Volo.Abp.DependencyInjection;

namespace PerryQBot.CommandHandlers;

[Command("骰子")]
[ExposeServices(typeof(ICommandHandler))]
public class DiceCommandHandler : CommandHandlerBase
{
    public override MessageChainBuilder OnMessageChainBuilding(MessageChainBuilder builder)
    {
        var number = new Random(DateTime.Now.Millisecond).Next(1, 6);
        return builder.Dice(number.ToString());
    }
}