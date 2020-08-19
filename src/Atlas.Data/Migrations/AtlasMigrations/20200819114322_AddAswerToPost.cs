using Microsoft.EntityFrameworkCore.Migrations;

namespace Atlas.Data.Migrations.AtlasMigrations
{
    public partial class AddAswerToPost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasAnswer",
                table: "Post",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAnswer",
                table: "Post",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasAnswer",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "IsAnswer",
                table: "Post");
        }
    }
}
