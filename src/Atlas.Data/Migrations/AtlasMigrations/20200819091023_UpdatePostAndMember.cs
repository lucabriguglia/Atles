using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Atlas.Data.Migrations.AtlasMigrations
{
    public partial class UpdatePostAndMember : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_Member_MemberId",
                table: "Post");

            migrationBuilder.RenameColumn(
                name: "TimeStamp",
                table: "Post",
                newName: "CreatedOn");

            migrationBuilder.RenameColumn(
                name: "MemberId",
                table: "Post",
                newName: "CreatedBy");

            migrationBuilder.RenameIndex(
                name: "IX_Post_MemberId",
                table: "Post",
                newName: "IX_Post_CreatedBy");

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                table: "Post",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "Post",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeStamp",
                table: "Member",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Post_ModifiedBy",
                table: "Post",
                column: "ModifiedBy");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_Member_CreatedBy",
                table: "Post");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_Member_ModifiedBy",
                table: "Post");

            migrationBuilder.DropIndex(
                name: "IX_Post_ModifiedBy",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "TimeStamp",
                table: "Member");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "Post",
                newName: "TimeStamp");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Post",
                newName: "MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_Post_CreatedBy",
                table: "Post",
                newName: "IX_Post_MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Member_MemberId",
                table: "Post",
                column: "MemberId",
                principalTable: "Member",
                principalColumn: "Id");
        }
    }
}
