using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerryQBot.Migrations
{
    /// <inheritdoc />
    public partial class init4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DialogCollection_UserQQ",
                table: "DialogCollection");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_DialogCollection_UserQQ",
                table: "DialogCollection",
                column: "UserQQ",
                unique: true);
        }
    }
}