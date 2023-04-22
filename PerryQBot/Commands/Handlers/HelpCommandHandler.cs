using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace PerryQBot.Commands.Handlers;

[Command("help")]
[Command("帮助")]
[ExposeServices(typeof(ICommandHandler))]
public class HelpCommandHandler : CommandHandlerBase
{
    public override async Task<string> HandleAndResponseAsync(CommandContext context)
    {
        await Task.CompletedTask;
        var cmd = BotOptions.Value.CommandPrefix;
        return $"""
                以下是支持的命令
                {cmd}帮助
                {cmd}代码
                {cmd}清空历史
                {cmd}预设 我想让你充当英英词典，对于给出的英文单词，你要给出其中文意思以及英文解释，此外不要有其他反馈，第一个单词是“Hello"。
                """;
    }
}