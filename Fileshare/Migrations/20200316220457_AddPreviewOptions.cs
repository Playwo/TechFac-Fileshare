using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Fileshare.Migrations
{
    public partial class AddPreviewOptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PreviewOptions",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    Redirection = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreviewOptions", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_PreviewOptions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PreviewOptions");
        }
    }
}
