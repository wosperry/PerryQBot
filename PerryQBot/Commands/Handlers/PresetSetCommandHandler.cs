using Volo.Abp.DependencyInjection;

[Command("修改预设", "修改我的预设")]
public class PresetSetCommandHandler : CommandHandlerBase
{
    public override string HandleAndResponseAsync(CommandContext context)
    {
        return $"您的预设成功修改为：{this.GetMessageString(context.Message)}";
    }
}