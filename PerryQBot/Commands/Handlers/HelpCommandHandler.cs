using Mirai.Net.Data.Messages;
using Mirai.Net.Sessions.Http.Managers;
using Volo.Abp.DependencyInjection;

public class HelpCommandHandler : CommandHandlerBase, ITransientDependency
{
    public override string GetCommand() => "#帮助";

    public override string GetCommandDescription() => "获取帮助信息";

    public override string GetResponseMessage(CommandContext context) =>
    """
        #帮助：获取帮助信息
        #预设：修改个人的预设
    """;
}