using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelloWorldAPI.Data.Migrations
{
    public partial class AddRepliesToComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentUser_AspNetUsers_CommentLikedId",
                table: "CommentUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommentUser",
                table: "CommentUser");

            migrationBuilder.DropIndex(
                name: "IX_CommentUser_CommentsLikedId",
                table: "CommentUser");

            migrationBuilder.RenameColumn(
                name: "CommentLikedId",
                table: "CommentUser",
                newName: "UserLikedId");

            migrationBuilder.AddColumn<Guid>(
                name: "CommentId",
                table: "Comments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommentUser",
                table: "CommentUser",
                columns: new[] { "CommentsLikedId", "UserLikedId" });

            migrationBuilder.CreateTable(
                name: "Reply",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RepliedOnId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reply", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reply_Reply_RepliedOnId",
                        column: x => x.RepliedOnId,
                        principalTable: "Reply",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentUser_UserLikedId",
                table: "CommentUser",
                column: "UserLikedId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CommentId",
                table: "Comments",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Reply_RepliedOnId",
                table: "Reply",
                column: "RepliedOnId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_CommentId",
                table: "Comments",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentUser_AspNetUsers_UserLikedId",
                table: "CommentUser",
                column: "UserLikedId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_CommentId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentUser_AspNetUsers_UserLikedId",
                table: "CommentUser");

            migrationBuilder.DropTable(
                name: "Reply");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommentUser",
                table: "CommentUser");

            migrationBuilder.DropIndex(
                name: "IX_CommentUser_UserLikedId",
                table: "CommentUser");

            migrationBuilder.DropIndex(
                name: "IX_Comments_CommentId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "CommentId",
                table: "Comments");

            migrationBuilder.RenameColumn(
                name: "UserLikedId",
                table: "CommentUser",
                newName: "CommentLikedId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommentUser",
                table: "CommentUser",
                columns: new[] { "CommentLikedId", "CommentsLikedId" });

            migrationBuilder.CreateIndex(
                name: "IX_CommentUser_CommentsLikedId",
                table: "CommentUser",
                column: "CommentsLikedId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentUser_AspNetUsers_CommentLikedId",
                table: "CommentUser",
                column: "CommentLikedId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
