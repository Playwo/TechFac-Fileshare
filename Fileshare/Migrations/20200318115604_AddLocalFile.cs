using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Fileshare.Migrations
{
    public partial class AddLocalFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Uploads_Filename",
                table: "Uploads");

            migrationBuilder.DropColumn(
                name: "Filename",
                table: "Uploads");

            migrationBuilder.AddColumn<string>(
                name: "Extension",
                table: "Uploads",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FileId",
                table: "Uploads",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Uploads",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LocalFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Checksum = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalFiles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Uploads_FileId",
                table: "Uploads",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_Uploads_Name",
                table: "Uploads",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocalFiles_Checksum",
                table: "LocalFiles",
                column: "Checksum",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Uploads_LocalFiles_FileId",
                table: "Uploads",
                column: "FileId",
                principalTable: "LocalFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Uploads_LocalFiles_FileId",
                table: "Uploads");

            migrationBuilder.DropTable(
                name: "LocalFiles");

            migrationBuilder.DropIndex(
                name: "IX_Uploads_FileId",
                table: "Uploads");

            migrationBuilder.DropIndex(
                name: "IX_Uploads_Name",
                table: "Uploads");

            migrationBuilder.DropColumn(
                name: "Extension",
                table: "Uploads");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "Uploads");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Uploads");

            migrationBuilder.AddColumn<string>(
                name: "Filename",
                table: "Uploads",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Uploads_Filename",
                table: "Uploads",
                column: "Filename",
                unique: true);
        }
    }
}
