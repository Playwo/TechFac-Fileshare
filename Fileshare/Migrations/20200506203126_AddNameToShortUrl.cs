using Microsoft.EntityFrameworkCore.Migrations;

namespace Fileshare.Migrations
{
    public partial class AddNameToShortUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ShortUrls",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "ShortUrls");
        }
    }
}
