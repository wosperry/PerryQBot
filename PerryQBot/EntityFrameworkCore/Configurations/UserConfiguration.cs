﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerryQBot.EntityFrameworkCore.Entities;

namespace PerryQBot.EntityFrameworkCore.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(nameof(User));
        builder.HasIndex(x => x.QQ).IsUnique();
        builder.Property(x => x.QQ).HasMaxLength(20);
        builder.Property(x => x.QQNickName).HasMaxLength(500);
        builder.Property(x => x.Preset).HasMaxLength(5000);
    }
}