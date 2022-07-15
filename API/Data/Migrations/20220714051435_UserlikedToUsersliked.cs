using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    public partial class UserlikedToUsersliked : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleUser_AspNetUsers_UserLikedId",
                table: "ArticleUser");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentUser_AspNetUsers_UserLikedId",
                table: "CommentUser");

            migrationBuilder.DropForeignKey(
                name: "FK_PostUser_AspNetUsers_UserLikedId",
                table: "PostUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUser_AspNetUsers_UserLikedId",
                table: "ProjectUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ReplyUser_AspNetUsers_UserLikedId",
                table: "ReplyUser");

            migrationBuilder.RenameColumn(
                name: "UserLikedId",
                table: "ReplyUser",
                newName: "UsersLikedId");

            migrationBuilder.RenameIndex(
                name: "IX_ReplyUser_UserLikedId",
                table: "ReplyUser",
                newName: "IX_ReplyUser_UsersLikedId");

            migrationBuilder.RenameColumn(
                name: "UserLikedId",
                table: "ProjectUser",
                newName: "UsersLikedId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectUser_UserLikedId",
                table: "ProjectUser",
                newName: "IX_ProjectUser_UsersLikedId");

            migrationBuilder.RenameColumn(
                name: "UserLikedId",
                table: "PostUser",
                newName: "UsersLikedId");

            migrationBuilder.RenameIndex(
                name: "IX_PostUser_UserLikedId",
                table: "PostUser",
                newName: "IX_PostUser_UsersLikedId");

            migrationBuilder.RenameColumn(
                name: "UserLikedId",
                table: "CommentUser",
                newName: "UsersLikedId");

            migrationBuilder.RenameIndex(
                name: "IX_CommentUser_UserLikedId",
                table: "CommentUser",
                newName: "IX_CommentUser_UsersLikedId");

            migrationBuilder.RenameColumn(
                name: "UserLikedId",
                table: "ArticleUser",
                newName: "UsersLikedId");

            migrationBuilder.RenameIndex(
                name: "IX_ArticleUser_UserLikedId",
                table: "ArticleUser",
                newName: "IX_ArticleUser_UsersLikedId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleUser_AspNetUsers_UsersLikedId",
                table: "ArticleUser",
                column: "UsersLikedId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentUser_AspNetUsers_UsersLikedId",
                table: "CommentUser",
                column: "UsersLikedId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostUser_AspNetUsers_UsersLikedId",
                table: "PostUser",
                column: "UsersLikedId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUser_AspNetUsers_UsersLikedId",
                table: "ProjectUser",
                column: "UsersLikedId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReplyUser_AspNetUsers_UsersLikedId",
                table: "ReplyUser",
                column: "UsersLikedId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleUser_AspNetUsers_UsersLikedId",
                table: "ArticleUser");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentUser_AspNetUsers_UsersLikedId",
                table: "CommentUser");

            migrationBuilder.DropForeignKey(
                name: "FK_PostUser_AspNetUsers_UsersLikedId",
                table: "PostUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUser_AspNetUsers_UsersLikedId",
                table: "ProjectUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ReplyUser_AspNetUsers_UsersLikedId",
                table: "ReplyUser");

            migrationBuilder.RenameColumn(
                name: "UsersLikedId",
                table: "ReplyUser",
                newName: "UserLikedId");

            migrationBuilder.RenameIndex(
                name: "IX_ReplyUser_UsersLikedId",
                table: "ReplyUser",
                newName: "IX_ReplyUser_UserLikedId");

            migrationBuilder.RenameColumn(
                name: "UsersLikedId",
                table: "ProjectUser",
                newName: "UserLikedId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectUser_UsersLikedId",
                table: "ProjectUser",
                newName: "IX_ProjectUser_UserLikedId");

            migrationBuilder.RenameColumn(
                name: "UsersLikedId",
                table: "PostUser",
                newName: "UserLikedId");

            migrationBuilder.RenameIndex(
                name: "IX_PostUser_UsersLikedId",
                table: "PostUser",
                newName: "IX_PostUser_UserLikedId");

            migrationBuilder.RenameColumn(
                name: "UsersLikedId",
                table: "CommentUser",
                newName: "UserLikedId");

            migrationBuilder.RenameIndex(
                name: "IX_CommentUser_UsersLikedId",
                table: "CommentUser",
                newName: "IX_CommentUser_UserLikedId");

            migrationBuilder.RenameColumn(
                name: "UsersLikedId",
                table: "ArticleUser",
                newName: "UserLikedId");

            migrationBuilder.RenameIndex(
                name: "IX_ArticleUser_UsersLikedId",
                table: "ArticleUser",
                newName: "IX_ArticleUser_UserLikedId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleUser_AspNetUsers_UserLikedId",
                table: "ArticleUser",
                column: "UserLikedId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentUser_AspNetUsers_UserLikedId",
                table: "CommentUser",
                column: "UserLikedId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostUser_AspNetUsers_UserLikedId",
                table: "PostUser",
                column: "UserLikedId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUser_AspNetUsers_UserLikedId",
                table: "ProjectUser",
                column: "UserLikedId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReplyUser_AspNetUsers_UserLikedId",
                table: "ReplyUser",
                column: "UserLikedId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
