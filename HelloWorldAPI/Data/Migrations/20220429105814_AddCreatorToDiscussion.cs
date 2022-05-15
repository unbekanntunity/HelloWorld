using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelloWorldAPI.Data.Migrations
{
    public partial class AddCreatorToDiscussion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionUsers_AspNetUsers_UserId",
                table: "DiscussionUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionUsers_Discussions_DiscussionId",
                table: "DiscussionUsers");

            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                table: "Discussions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Discussions_CreatorId",
                table: "Discussions",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Discussions_AspNetUsers_CreatorId",
                table: "Discussions",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Discussions_AspNetUsers_CreatorId",
                table: "Discussions");

            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionUsers_AspNetUsers_UserId",
                table: "DiscussionUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionUsers_Discussions_DiscussionId",
                table: "DiscussionUsers");

            migrationBuilder.DropIndex(
                name: "IX_Discussions_CreatorId",
                table: "Discussions");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Discussions");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionUsers_AspNetUsers_UserId",
                table: "DiscussionUsers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionUsers_Discussions_DiscussionId",
                table: "DiscussionUsers",
                column: "DiscussionId",
                principalTable: "Discussions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
