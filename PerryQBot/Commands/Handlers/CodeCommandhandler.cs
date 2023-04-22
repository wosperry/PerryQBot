using Volo.Abp.DependencyInjection;

namespace PerryQBot.Commands.Handlers;

[Command("代码")]
[Command("代碼")]
[Command("源码")]
[Command("源碼")]
[Command("原始碼")]
[Command("源代码")]
[Command("github")]
[Command("gitee")]
[ExposeServices(typeof(ICommandHandler))]
public class CodeCommandhandler : CommandHandlerBase
{
    public override async Task<string> HandleAndResponseAsync(CommandContext context)
    {
        await Task.CompletedTask;
        return """
            项目地址：
            - Gitee: https://gitee.com/wosperry/PerryQBot
            - Github: https://github.com/wosperry/PerryQBot
            """;
    }
}