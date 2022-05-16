using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelloWorldAPI.Data.Migrations
{
    public partial class AddRepliesForArticles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RepliedOnArticleId",
                table: "Replies",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Replies_RepliedOnArticleId",
                table: "Replies",
                column: "RepliedOnArticleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Replies_Articles_RepliedOnArticleId",
                table: "Replies",
                column: "RepliedOnArticleId",
                principalTable: "Articles",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Replies_Articles_RepliedOnArticleId",
                table: "Replies");

            migrationBuilder.DropIndex(
                name: "IX_Replies_RepliedOnArticleId",
                table: "Replies");

            migrationBuilder.DropColumn(
                name: "RepliedOnArticleId",
                table: "Replies");
        }
    }
}
