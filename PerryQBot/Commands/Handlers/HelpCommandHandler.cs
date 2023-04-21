using Volo.Abp.DependencyInjection;

[Command("帮助", "获取帮助信息")]
public class HelpCommandHandler : CommandHandlerBase
{
    public override string HandleAndResponseAsync(CommandContext context)
    {
        return """
            以下是支持的命令
            #帮助
            #修改预设 我想让你充当英英词典，对于给出的英文单词，你要给出其中文意思以及英文解释，并且给出一个例句，此外不要有其他反馈，第一个单词是“Hello"。
            #获取预设
            """;
    }
}