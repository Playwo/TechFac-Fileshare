using Microsoft.EntityFrameworkCore.Migrations;

namespace Fileshare.Migrations
{
    public partial class ImprovePreviewOptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Redirection",
                table: "PreviewOptions");

            migrationBuilder.AddColumn<bool>(
                name: "RedirectAgents",
                table: "PreviewOptions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RedirectCategories",
                table: "PreviewOptions",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RedirectAgents",
                table: "PreviewOptions");

            migrationBuilder.DropColumn(
                name: "RedirectCategories",
                table: "PreviewOptions");

            migrationBuilder.AddColumn<int>(
                name: "Redirection",
                table: "PreviewOptions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
