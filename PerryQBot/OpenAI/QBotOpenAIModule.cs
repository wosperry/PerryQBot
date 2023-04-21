using PerryQBot.Options;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

[DependsOn(
    typeof(AbpBackgroundJobsModule)
)]
public class QBotOpenAIModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        Configure<OpenAiOptions>(configuration.GetSection("OpenAiOptions"));
    }
}