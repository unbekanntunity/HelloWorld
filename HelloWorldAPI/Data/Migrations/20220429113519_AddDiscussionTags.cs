using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelloWorldAPI.Data.Migrations
{
    public partial class AddDiscussionTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiscussionTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DiscussionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscussionTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscussionTags_Discussions_DiscussionId",
                        column: x => x.DiscussionId,
                        principalTable: "Discussions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DiscussionTags_Tag_TagName",
                        column: x => x.TagName,
                        principalTable: "Tag",
                        principalColumn: "Name");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionTags_DiscussionId",
                table: "DiscussionTags",
                column: "DiscussionId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionTags_TagName",
                table: "DiscussionTags",
                column: "TagName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscussionTags");
        }
    }
}
