using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerryQBot.EntityFrameworkCore.Entities;

namespace PerryQBot.EntityFrameworkCore.Configurations;

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