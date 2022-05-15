using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelloWorldAPI.Data.Migrations
{
    public partial class ChangeNaming : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUser_AspNetUsers_ProjectLikedId",
                table: "ProjectUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectUser",
                table: "ProjectUser");

            migrationBuilder.DropIndex(
                name: "IX_ProjectUser_ProjectsLikedId",
                table: "ProjectUser");

            migrationBuilder.RenameColumn(
                name: "ProjectLikedId",
                table: "ProjectUser",
                newName: "UserLikedId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectUser",
                table: "ProjectUser",
                columns: new[] { "ProjectsLikedId", "UserLikedId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUser_UserLikedId",
                table: "ProjectUser",
                column: "UserLikedId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUser_AspNetUsers_UserLikedId",
                table: "ProjectUser",
                column: "UserLikedId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUser_AspNetUsers_UserLikedId",
                table: "ProjectUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectUser",
                table: "ProjectUser");

            migrationBuilder.DropIndex(
                name: "IX_ProjectUser_UserLikedId",
                table: "ProjectUser");

            migrationBuilder.RenameColumn(
                name: "UserLikedId",
                table: "ProjectUser",
                newName: "ProjectLikedId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectUser",
                table: "ProjectUser",
                columns: new[] { "ProjectLikedId", "ProjectsLikedId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUser_ProjectsLikedId",
                table: "ProjectUser",
                column: "ProjectsLikedId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUser_AspNetUsers_ProjectLikedId",
                table: "ProjectUser",
                column: "ProjectLikedId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
