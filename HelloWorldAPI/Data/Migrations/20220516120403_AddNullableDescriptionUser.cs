using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelloWorldAPI.Data.Migrations
{
    public partial class AddNullableDescriptionUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscussionUser");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiscussionUser",
                columns: table => new
                {
                    DiscussionsJoinedId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscussionUser", x => new { x.DiscussionsJoinedId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_DiscussionUser_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiscussionUser_Discussions_DiscussionsJoinedId",
                        column: x => x.DiscussionsJoinedId,
                        principalTable: "Discussions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionUser_UsersId",
                table: "DiscussionUser",
                column: "UsersId");
        }
    }
}
