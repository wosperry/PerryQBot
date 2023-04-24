using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerryQBot.Migrations
{
    /// <inheritdoc />
    public partial class Init1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DialogCollectionItem_DialogCollection_DialogCollectionId",
                table: "DialogCollectionItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DialogCollection",
                table: "DialogCollection");

            migrationBuilder.DropColumn(
                name: "From",
                table: "DialogCollectionItem");

            migrationBuilder.DropColumn(
                name: "Time",
                table: "DialogCollectionItem");

            migrationBuilder.DropColumn(
                name: "To",
                table: "DialogCollectionItem");

            migrationBuilder.RenameTable(
                name: "DialogCollection",
                newName: "DialogCollections");

            migrationBuilder.RenameColumn(
                name: "CollectorQQ",
                table: "DialogCollections",
                newName: "UserQQ");

            migrationBuilder.RenameColumn(
                name: "CollectorMessage",
                table: "DialogCollections",
                newName: "Message");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "DialogCollectionItem",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTime",
                table: "DialogCollections",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "DialogCollections",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DialogCollectionItem_DialogCollections_DialogCollectionId",
                table: "DialogCollectionItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DialogCollections",
                table: "DialogCollections");

            migrationBuilder.DropColumn(
                name: "DateTime",
                table: "DialogCollections");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "DialogCollections");

            migrationBuilder.RenameTable(
                name: "DialogCollections",
                newName: "DialogCollection");

            migrationBuilder.RenameColumn(
                name: "UserQQ",
                table: "DialogCollection",
                newName: "CollectorQQ");

            migrationBuilder.RenameColumn(
                name: "Message",
                table: "DialogCollection",
                newName: "CollectorMessage");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "DialogCollectionItem",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(5000)",
                oldMaxLength: 5000,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "From",
                table: "DialogCollectionItem",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Time",
                table: "DialogCollectionItem",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "To",
                table: "DialogCollectionItem",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DialogCollection",
                table: "DialogCollection",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DialogCollectionItem_DialogCollection_DialogCollectionId",
                table: "DialogCollectionItem",
                column: "DialogCollectionId",
                principalTable: "DialogCollection",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
