using Microsoft.EntityFrameworkCore.Migrations;

namespace Atlify.Data.Migrations.AtlifyMigrations
{
    public partial class AddSlugToForumAndPost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Post",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Forum",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Forum");
        }
    }
}
