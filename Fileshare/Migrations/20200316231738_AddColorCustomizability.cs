using Microsoft.EntityFrameworkCore.Migrations;

namespace Fileshare.Migrations
{
    public partial class AddColorCustomizability : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BackgroundColor",
                table: "PreviewOptions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BoxColor",
                table: "PreviewOptions",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackgroundColor",
                table: "PreviewOptions");

            migrationBuilder.DropColumn(
                name: "BoxColor",
                table: "PreviewOptions");
        }
    }
}
