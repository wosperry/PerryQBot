using Mirai.Net.Sessions;
using PerryQBot.Options;
using Volo.Abp;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.BackgroundWorkers.Quartz;
using Volo.Abp.Modularity;

namespace PerryQBot.QQBot;

[DependsOn(
    typeof(AbpBackgroundWorkersModule),
    typeof(AbpBackgroundWorkersQuartzModule)
)]
public class QBotMiraiQQModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        var botOptions = new MiraiBotOptions();
        var botOptionsSection = configuration.GetSection("MiraiBotOptions");
        botOptionsSection.Bind(botOptions);

        context.Services.AddSingleton(new MiraiBot
        {
            Address = $"{botOptions.Host}:{botOptions.Port}",
            QQ = $"{botOptions.QQ}",
            VerifyKey = botOptions.VerifyKey
        });

        Configure<MiraiBotOptions>(botOptionsSection);
        Configure<NewsOptions>(configuration.GetSection(nameof(NewsOptions)));
    }

    public override async void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        await context.AddBackgroundWorkerAsync<QBotBackgroundWorker>();
    }
}