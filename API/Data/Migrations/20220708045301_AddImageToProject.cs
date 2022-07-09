using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    public partial class AddImageToProject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "ImageUrls",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImageUrls_ProjectId",
                table: "ImageUrls",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageUrls_Projects_ProjectId",
                table: "ImageUrls",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageUrls_Projects_ProjectId",
                table: "ImageUrls");

            migrationBuilder.DropIndex(
                name: "IX_ImageUrls_ProjectId",
                table: "ImageUrls");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "ImageUrls");
        }
    }
}
