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
    }
}

public class UserHistoryConfiguration : IEntityTypeConfiguration<UserHistory>
{
    public void Configure(EntityTypeBuilder<UserHistory> builder)
    {
        builder.HasOne(uh => uh.User)
          .WithMany(u => u.History)
          .HasForeignKey(uh => uh.UserId)
          .OnDelete(DeleteBehavior.Cascade); // 开启级联删除
    }
}