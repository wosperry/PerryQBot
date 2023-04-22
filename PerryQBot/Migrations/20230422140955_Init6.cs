using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PerryQBot.Migrations
{
    /// <inheritdoc />
    public partial class Init6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DialogCollectionItem");

            migrationBuilder.AddColumn<string>(
                name: "QuoteMessage",
                table: "DialogCollection",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuoteMessage",
                table: "DialogCollection");

            migrationBuilder.CreateTable(
                name: "DialogCollectionItem",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DialogCollectionId = table.Column<long>(type: "bigint", nullable: false),
                    Message = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DialogCollectionItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DialogCollectionItem_DialogCollection_DialogCollectionId",
                        column: x => x.DialogCollectionId,
                        principalTable: "DialogCollection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DialogCollectionItem_DialogCollectionId",
                table: "DialogCollectionItem",
                column: "DialogCollectionId");
        }
    }
}