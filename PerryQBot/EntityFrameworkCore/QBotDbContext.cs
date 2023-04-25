using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using PerryQBot.EntityFrameworkCore.Entities;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

[ConnectionStringName("Default")]
public class QBotDbContext : AbpDbContext<QBotDbContext>
{
    public DbSet<User> Users { get; set; }
    public DbSet<DialogCollection> DialogCollections { get; set; }

    public QBotDbContext(DbContextOptions<QBotDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(QBotDbContext).Assembly);
    }
}