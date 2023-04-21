using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.PostgreSql;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;

namespace PerryQBot.EntityFrameworkCore
{
    [DependsOn(
        typeof(AbpEntityFrameworkCorePostgreSqlModule),
        typeof(AbpUnitOfWorkModule)
    )]
    public class QBotEntityFrameworkModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            context.Services.AddAbpDbContext<QBotDbContext>(options =>
            {
                options.AddDefaultRepositories(true);
            });
            Configure<AbpDbContextOptions>(options =>
            {
                options.Configure(opts =>
                {
                    opts.UseNpgsql();
                });
            });
        }
    }
}