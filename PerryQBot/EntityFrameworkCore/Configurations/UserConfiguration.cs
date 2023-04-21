using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using PerryQBot.EntityFrameworkCore.Entities;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(nameof(User));
        builder.HasIndex(x => x.QQ).IsUnique();
        builder.Property(x => x.QQ).HasMaxLength(20);
        builder.Property(x => x.QQNickName).HasMaxLength(500);
        builder.Property(x => x.Preset).HasMaxLength(500);
        // TODO: 写死3条，后面再看
        builder.Property(x => x.History).HasConversion(
            v => JsonConvert.SerializeObject(v.Take(3)),
            v => JsonConvert.DeserializeObject<List<UserHistory>>(!string.IsNullOrWhiteSpace(v) ? v : "[]")
        );
    }
}