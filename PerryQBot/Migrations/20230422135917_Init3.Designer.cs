﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Volo.Abp.EntityFrameworkCore;

#nullable disable

namespace PerryQBot.Migrations
{
    [DbContext(typeof(QBotDbContext))]
    [Migration("20230422135917_Init3")]
    partial class Init3
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("_Abp_DatabaseProvider", EfCoreDatabaseProvider.PostgreSql)
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PerryQBot.EntityFrameworkCore.Entities.DialogCollection", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Message")
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.Property<string>("UserName")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("UserQQ")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.HasKey("Id");

                    b.HasIndex("UserQQ")
                        .IsUnique();

                    b.ToTable("DialogCollection", (string)null);
                });

            modelBuilder.Entity("PerryQBot.EntityFrameworkCore.Entities.DialogCollectionItem", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("DialogCollectionId")
                        .HasColumnType("bigint");

                    b.Property<string>("Message")
                        .HasMaxLength(5000)
                        .HasColumnType("character varying(5000)");

                    b.HasKey("Id");

                    b.HasIndex("DialogCollectionId");

                    b.ToTable("DialogCollectionItem");
                });

            modelBuilder.Entity("PerryQBot.EntityFrameworkCore.Entities.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Preset")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<string>("QQ")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<string>("QQNickName")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.HasKey("Id");

                    b.HasIndex("QQ")
                        .IsUnique();

                    b.ToTable("User", (string)null);
                });

            modelBuilder.Entity("PerryQBot.EntityFrameworkCore.Entities.UserHistory", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Message")
                        .HasColumnType("text");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserHistory");
                });

            modelBuilder.Entity("PerryQBot.EntityFrameworkCore.Entities.DialogCollectionItem", b =>
                {
                    b.HasOne("PerryQBot.EntityFrameworkCore.Entities.DialogCollection", "DialogCollection")
                        .WithMany("Items")
                        .HasForeignKey("DialogCollectionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DialogCollection");
                });

            modelBuilder.Entity("PerryQBot.EntityFrameworkCore.Entities.UserHistory", b =>
                {
                    b.HasOne("PerryQBot.EntityFrameworkCore.Entities.User", "User")
                        .WithMany("History")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("PerryQBot.EntityFrameworkCore.Entities.DialogCollection", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("PerryQBot.EntityFrameworkCore.Entities.User", b =>
                {
                    b.Navigation("History");
                });
#pragma warning restore 612, 618
        }
    }
}
