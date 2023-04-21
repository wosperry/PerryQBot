using Volo.Abp.DependencyInjection;

[Command("帮助", "获取帮助信息")]
public class HelpCommandHandler : CommandHandlerBase, ITransientDependency
{
    public override string HandleAndResponseAsync(CommandContext context)
    {
        return """
               以下是支持的命令写法，方括号内表示内容，方括号外是说明
                   #帮助
                   #修改预设 你的名字叫张三。
                   #获取预设
               """;
    }
}