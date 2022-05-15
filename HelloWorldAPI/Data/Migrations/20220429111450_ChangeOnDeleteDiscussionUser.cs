using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelloWorldAPI.Data.Migrations
{
    public partial class ChangeOnDeleteDiscussionUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionUsers_AspNetUsers_UserId",
                table: "DiscussionUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionUsers_Discussions_DiscussionId",
                table: "DiscussionUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionUsers_AspNetUsers_UserId",
                table: "DiscussionUsers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionUsers_Discussions_DiscussionId",
                table: "DiscussionUsers",
                column: "DiscussionId",
                principalTable: "Discussions",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionUsers_AspNetUsers_UserId",
                table: "DiscussionUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionUsers_Discussions_DiscussionId",
                table: "DiscussionUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionUsers_AspNetUsers_UserId",
                table: "DiscussionUsers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionUsers_Discussions_DiscussionId",
                table: "DiscussionUsers",
                column: "DiscussionId",
                principalTable: "Discussions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
