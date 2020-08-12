using Microsoft.EntityFrameworkCore.Migrations;

namespace Atlas.Data.Migrations.AtlasMigrations
{
    public partial class AddSiteLanguage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "Site",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Language",
                table: "Site");
        }
    }
}
