using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Atlas.Data.Migrations.AtlasMigrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Member",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    DisplayName = table.Column<string>(nullable: true),
                    TopicsCount = table.Column<int>(nullable: false),
                    RepliesCount = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Member", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PermissionSet",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SiteId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionSet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Site",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Site", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SiteId = table.Column<Guid>(nullable: false),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    TargetId = table.Column<Guid>(nullable: false),
                    TargetType = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Data = table.Column<string>(nullable: true),
                    MemberId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Event_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Permission",
                columns: table => new
                {
                    PermissionSetId = table.Column<Guid>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => new { x.PermissionSetId, x.RoleId, x.Type });
                    table.ForeignKey(
                        name: "FK_Permission_PermissionSet_PermissionSetId",
                        column: x => x.PermissionSetId,
                        principalTable: "PermissionSet",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SiteId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    SortOrder = table.Column<int>(nullable: false),
                    TopicsCount = table.Column<int>(nullable: false),
                    RepliesCount = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    PermissionSetId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Category_PermissionSet_PermissionSetId",
                        column: x => x.PermissionSetId,
                        principalTable: "PermissionSet",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Category_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Forum",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CategoryId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    SortOrder = table.Column<int>(nullable: false),
                    TopicsCount = table.Column<int>(nullable: false),
                    RepliesCount = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    PermissionSetId = table.Column<Guid>(nullable: true),
                    LastPostId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forum", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Forum_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Forum_PermissionSet_PermissionSetId",
                        column: x => x.PermissionSetId,
                        principalTable: "PermissionSet",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TopicId = table.Column<Guid>(nullable: true),
                    ForumId = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    RepliesCount = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    MemberId = table.Column<Guid>(nullable: false),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    LastReplyId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Post_Forum_ForumId",
                        column: x => x.ForumId,
                        principalTable: "Forum",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Post_Post_LastReplyId",
                        column: x => x.LastReplyId,
                        principalTable: "Post",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Post_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Post_Post_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Post",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Category_PermissionSetId",
                table: "Category",
                column: "PermissionSetId");

            migrationBuilder.CreateIndex(
                name: "IX_Category_SiteId",
                table: "Category",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_MemberId",
                table: "Event",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Forum_CategoryId",
                table: "Forum",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Forum_LastPostId",
                table: "Forum",
                column: "LastPostId");

            migrationBuilder.CreateIndex(
                name: "IX_Forum_PermissionSetId",
                table: "Forum",
                column: "PermissionSetId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_ForumId",
                table: "Post",
                column: "ForumId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_LastReplyId",
                table: "Post",
                column: "LastReplyId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_MemberId",
                table: "Post",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_TopicId",
                table: "Post",
                column: "TopicId");

            migrationBuilder.AddForeignKey(
                name: "FK_Forum_Post_LastPostId",
                table: "Forum",
                column: "LastPostId",
                principalTable: "Post",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Category_PermissionSet_PermissionSetId",
                table: "Category");

            migrationBuilder.DropForeignKey(
                name: "FK_Forum_PermissionSet_PermissionSetId",
                table: "Forum");

            migrationBuilder.DropForeignKey(
                name: "FK_Category_Site_SiteId",
                table: "Category");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_Member_MemberId",
                table: "Post");

            migrationBuilder.DropForeignKey(
                name: "FK_Forum_Category_CategoryId",
                table: "Forum");

            migrationBuilder.DropForeignKey(
                name: "FK_Forum_Post_LastPostId",
                table: "Forum");

            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.DropTable(
                name: "Permission");

            migrationBuilder.DropTable(
                name: "PermissionSet");

            migrationBuilder.DropTable(
                name: "Site");

            migrationBuilder.DropTable(
                name: "Member");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Post");

            migrationBuilder.DropTable(
                name: "Forum");
        }
    }
}
