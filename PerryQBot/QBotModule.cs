using Mirai.Net.Sessions;
using PerryQBot.Options;
using Volo.Abp;
using Volo.Abp.AspNetCore;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.DistributedLocking;
using Volo.Abp.EventBus;
using Volo.Abp.Modularity;

[DependsOn(
    typeof(AbpEventBusModule),
    typeof(AbpAspNetCoreModule),
    typeof(AbpBackgroundJobsModule),
    typeof(AbpBackgroundWorkersModule)
)]
public class QBotModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var botOptions = new MiraiBotOptions();
        var section = configuration.GetSection("MiraiBotOptions");
        section.Bind(botOptions);

        context.Services.AddSingleton(new MiraiBot
        {
            Address = $"{botOptions.Host}:{botOptions.Port}",
            QQ = $"{botOptions.QQ}",
            VerifyKey = botOptions.VerifyKey
        });

        Configure<OpenAiOptions>(configuration.GetSection("OpenAiOptions"));
    }

    public override async void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        await context.AddBackgroundWorkerAsync<QBotBackgroundWorker>();
    }
}