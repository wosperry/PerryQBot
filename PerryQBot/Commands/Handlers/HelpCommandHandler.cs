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

                介绍：
                PerryQBot是一个使用Mirai.Net类库对接Mirai的QQ机器人，可以自动处理QQ消息并回复。该项目旨在管理QQ用户或群聊中@机器人的人的消息，每个用户都有一个预设，默认为空。
                聊天中可以保持数条历史记录以实现连续对话，智能对话是通过 GPT-3.5-turbo 的接口实现的。
                该项目已在GitHub上公开，同时也在Gitee有开放的仓库，开源协议为AGPL3.0。如果您希望查看或贡献该项目，请点击以下链接访问：
                https://github.com/wosperry/PerryQBot
                感谢您的兴趣和支持！
                """;
    }
}