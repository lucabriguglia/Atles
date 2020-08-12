using Microsoft.EntityFrameworkCore.Migrations;

namespace Atlas.Data.Migrations.AtlasMigrations
{
    public partial class AddSitePrivacyAndTerms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Privacy",
                table: "Site",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Terms",
                table: "Site",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Privacy",
                table: "Site");

            migrationBuilder.DropColumn(
                name: "Terms",
                table: "Site");
        }
    }
}
