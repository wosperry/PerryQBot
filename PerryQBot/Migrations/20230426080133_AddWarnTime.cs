using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerryQBot.Migrations
{
    /// <inheritdoc />
    public partial class AddWarnTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WarnTime",
                table: "User",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WarnTime",
                table: "User");
        }
    }
}
