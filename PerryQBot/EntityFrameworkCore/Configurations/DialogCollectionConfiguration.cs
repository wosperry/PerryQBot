using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerryQBot.EntityFrameworkCore.Entities;

namespace PerryQBot.EntityFrameworkCore.Configurations;

public class DialogCollectionConfiguration : IEntityTypeConfiguration<DialogCollection>
{
    public void Configure(EntityTypeBuilder<DialogCollection> builder)
    {
        builder.ToTable(nameof(DialogCollection));
        builder.Property(x => x.UserName).HasMaxLength(50);
        builder.Property(x => x.UserQQ).HasMaxLength(20);
        builder.Property(x => x.Message).HasMaxLength(2000);
        builder.Property(x => x.QuoteMessage).HasMaxLength(2000);
        builder.Property(x => x.DateTime);
    }
}