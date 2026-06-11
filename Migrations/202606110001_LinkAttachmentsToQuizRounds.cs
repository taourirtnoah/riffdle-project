using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Riffdle.Data;

#nullable disable

namespace Riffdle.Migrations;

[DbContext(typeof(RiffdleDbContext))]
[Migration("202606110001_LinkAttachmentsToQuizRounds")]
public partial class LinkAttachmentsToQuizRounds : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "QuizRoundId",
            table: "Attachments",
            type: "int",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Attachments_QuizRoundId",
            table: "Attachments",
            column: "QuizRoundId");

        migrationBuilder.AddForeignKey(
            name: "FK_Attachments_QuizRounds_QuizRoundId",
            table: "Attachments",
            column: "QuizRoundId",
            principalTable: "QuizRounds",
            principalColumn: "Id");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Attachments_QuizRounds_QuizRoundId",
            table: "Attachments");

        migrationBuilder.DropIndex(
            name: "IX_Attachments_QuizRoundId",
            table: "Attachments");

        migrationBuilder.DropColumn(
            name: "QuizRoundId",
            table: "Attachments");
    }
}
