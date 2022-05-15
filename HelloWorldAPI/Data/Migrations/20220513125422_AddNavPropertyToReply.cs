using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelloWorldAPI.Data.Migrations
{
    public partial class AddNavPropertyToReply : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Replies_Comments_CommentId",
                table: "Replies");

            migrationBuilder.DropForeignKey(
                name: "FK_Replies_Replies_RepliedOnId",
                table: "Replies");

            migrationBuilder.RenameColumn(
                name: "RepliedOnId",
                table: "Replies",
                newName: "RepliedOnReplyId");

            migrationBuilder.RenameColumn(
                name: "CommentId",
                table: "Replies",
                newName: "RepliedOnCommentId");

            migrationBuilder.RenameIndex(
                name: "IX_Replies_RepliedOnId",
                table: "Replies",
                newName: "IX_Replies_RepliedOnReplyId");

            migrationBuilder.RenameIndex(
                name: "IX_Replies_CommentId",
                table: "Replies",
                newName: "IX_Replies_RepliedOnCommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Replies_Comments_RepliedOnCommentId",
                table: "Replies",
                column: "RepliedOnCommentId",
                principalTable: "Comments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Replies_Replies_RepliedOnReplyId",
                table: "Replies",
                column: "RepliedOnReplyId",
                principalTable: "Replies",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Replies_Comments_RepliedOnCommentId",
                table: "Replies");

            migrationBuilder.DropForeignKey(
                name: "FK_Replies_Replies_RepliedOnReplyId",
                table: "Replies");

            migrationBuilder.RenameColumn(
                name: "RepliedOnReplyId",
                table: "Replies",
                newName: "RepliedOnId");

            migrationBuilder.RenameColumn(
                name: "RepliedOnCommentId",
                table: "Replies",
                newName: "CommentId");

            migrationBuilder.RenameIndex(
                name: "IX_Replies_RepliedOnReplyId",
                table: "Replies",
                newName: "IX_Replies_RepliedOnId");

            migrationBuilder.RenameIndex(
                name: "IX_Replies_RepliedOnCommentId",
                table: "Replies",
                newName: "IX_Replies_CommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Replies_Comments_CommentId",
                table: "Replies",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Replies_Replies_RepliedOnId",
                table: "Replies",
                column: "RepliedOnId",
                principalTable: "Replies",
                principalColumn: "Id");
        }
    }
}
