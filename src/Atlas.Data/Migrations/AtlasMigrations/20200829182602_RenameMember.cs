using Microsoft.EntityFrameworkCore.Migrations;

namespace Atlas.Data.Migrations.AtlasMigrations
{
    public partial class RenameMember : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_Member_MemberId",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_Member_CreatedBy",
                table: "Post");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_Member_ModifiedBy",
                table: "Post");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Member",
                table: "Member");

            migrationBuilder.RenameTable(
                name: "Member",
                newName: "User");

            migrationBuilder.RenameColumn(
                name: "MemberId",
                table: "Event",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Event_MemberId",
                table: "Event",
                newName: "IX_Event_UserId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "User",
                newName: "IdentityUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_User_UserId",
                table: "Event",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_User_CreatedBy",
                table: "Post",
                column: "CreatedBy",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_User_ModifiedBy",
                table: "Post",
                column: "ModifiedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_User_UserId",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_User_CreatedBy",
                table: "Post");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_User_ModifiedBy",
                table: "Post");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "Member");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Event",
                newName: "MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_Event_UserId",
                table: "Event",
                newName: "IX_Event_MemberId");

            migrationBuilder.RenameColumn(
                name: "IdentityUserId",
                table: "Member",
                newName: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Member",
                table: "Member",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Member_MemberId",
                table: "Event",
                column: "MemberId",
                principalTable: "Member",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Member_CreatedBy",
                table: "Post",
                column: "CreatedBy",
                principalTable: "Member",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Member_ModifiedBy",
                table: "Post",
                column: "ModifiedBy",
                principalTable: "Member",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
