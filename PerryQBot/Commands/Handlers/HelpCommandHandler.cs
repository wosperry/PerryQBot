using Microsoft.Extensions.Options;
using PerryQBot.Options;
using Volo.Abp.DependencyInjection;

namespace PerryQBot.Commands.Handlers;

[Command("help")]
[Command("帮助")]
[ExposeServices(typeof(ICommandHandler))]
public class HelpCommandHandler : CommandHandlerBase
{
    public IOptions<MessageCollectionOptions> MessageCollectionOptions { get; set; }

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
                4. {cmd}收藏 [额外文本]
                   - 参数：额外文本
                   - 说明：收藏发送者引用的文本及当前发送的文本
                   - 示例：
                      - #收藏 你好，我的名字叫Perry。
                      - #收藏 我想让你充当英英词典，对于给出的英文单词，你要给出其中文意思以及英文解释，此外不要有其他反馈，第一个单词是“Hello"。
                      - [长按引用的前面的消息] #收藏 加上了我自己补充的内容
                5. {cmd}查询收藏 [条件]
                   - 说明：根据参数搜索查询收藏，最多返回展示{MessageCollectionOptions.Value.MaxResultCount}条结果
                   - 示例：
                      - #查询收藏 Perry
                      - #查询收藏 MongoDB
                """;
    }
}