using Volo.Abp.DependencyInjection;

[Command("获取预设", "查看我的预设")]
public class PresetGetCommandHandler : CommandHandlerBase
{
    public override string HandleAndResponseAsync(CommandContext context)
    {
        return $"您的预设为：hhh 骗你的，还没实现呢";
    }
}