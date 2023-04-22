using PerryQBot.EntityFrameworkCore;
using PerryQBot.Options;
using PerryQBot.QQBot;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;

namespace PerryQBot
{
    [DependsOn(
        typeof(AbpAspNetCoreMvcModule),
        typeof(AbpUnitOfWorkModule),
        typeof(AbpAutofacModule)
    )]
    [DependsOn(
        typeof(QBotOpenAIModule),
        typeof(QBotEntityFrameworkModule),
        typeof(QBotMiraiQQModule)
    )]
    public class QBotModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            Configure<MessageCollectionOptions>(configuration.GetSection(nameof(MessageCollectionOptions)));
        }
    }
}