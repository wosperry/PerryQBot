using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerryQBot.Migrations
{
    /// <inheritdoc />
    public partial class Init3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DialogCollectionItem_DialogCollections_DialogCollectionId",
                table: "DialogCollectionItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DialogCollections",
                table: "DialogCollections");

            migrationBuilder.RenameTable(
                name: "DialogCollections",
                newName: "DialogCollection");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DialogCollection",
                table: "DialogCollection",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DialogCollection_UserQQ",
                table: "DialogCollection",
                column: "UserQQ",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DialogCollectionItem_DialogCollection_DialogCollectionId",
                table: "DialogCollectionItem",
                column: "DialogCollectionId",
                principalTable: "DialogCollection",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DialogCollectionItem_DialogCollection_DialogCollectionId",
                table: "DialogCollectionItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DialogCollection",
                table: "DialogCollection");

            migrationBuilder.DropIndex(
                name: "IX_DialogCollection_UserQQ",
                table: "DialogCollection");

            migrationBuilder.RenameTable(
                name: "DialogCollection",
                newName: "DialogCollections");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DialogCollections",
                table: "DialogCollections",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DialogCollectionItem_DialogCollections_DialogCollectionId",
                table: "DialogCollectionItem",
                column: "DialogCollectionId",
                principalTable: "DialogCollections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
