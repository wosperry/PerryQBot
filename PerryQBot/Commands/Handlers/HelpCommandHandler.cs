using Volo.Abp.DependencyInjection;

namespace PerryQBot.Commands.Handlers;

[Command("帮助")]
[ExposeServices(typeof(ICommandHandler))]
public class HelpCommandHandler : CommandHandlerBase
{
    public override async Task<string> HandleAndResponseAsync(CommandContext context)
    {
        await Task.CompletedTask;
        return """
                以下是支持的命令
                #帮助
                #代码
                #清空历史
                #预设 我想让你充当英英词典，对于给出的英文单词，你要给出其中文意思以及英文解释，此外不要有其他反馈，第一个单词是“Hello"。
                """;
    }
}