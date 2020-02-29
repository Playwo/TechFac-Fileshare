using Microsoft.EntityFrameworkCore.Migrations;

namespace Fileshare.Migrations
{
    public partial class ContentType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Uploads",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Uploads_Filename",
                table: "Uploads",
                column: "Filename",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Uploads_Filename",
                table: "Uploads");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Uploads");
        }
    }
}
