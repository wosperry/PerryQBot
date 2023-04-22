using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace PerryQBot.Commands.Handlers;

[Command("help")]
[Command("帮助")]
[ExposeServices(typeof(ICommandHandler))]
public class HelpCommandHandler : CommandHandlerBase
{
    public override async Task ExecuteAsync(CommandContext context)
    {
        await Task.CompletedTask;
        var cmd = BotOptions.Value.CommandPrefix;
        ResponseMessage = $"""
                支持的命令：
                1. {cmd}帮助
                   - 说明：显示帮助信息
                2. {cmd}预设
                   - 参数：预设文本
                   - 说明：修改发送者的预设文本，同时清理发送者的历史消息
                   - 示例：
                      - #预设 你好，我的名字叫Perry。
                      - #预设 我想让你充当英英词典，对于给出的英文单词，你要给出其中文意思以及英文解释，此外不要有其他反馈，第一个单词是“Hello"。
                3. {cmd}清空历史
                   - 说明：清空历史记录（不包含预设）
                """;
    }
}