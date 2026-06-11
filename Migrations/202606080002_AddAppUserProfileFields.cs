using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Riffdle.Data;

#nullable disable

namespace Riffdle.Migrations;

[DbContext(typeof(RiffdleDbContext))]
[Migration("202606080002_AddAppUserProfileFields")]
public partial class AddAppUserProfileFields : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "OIB",
            table: "AspNetUsers",
            type: "nvarchar(11)",
            maxLength: 11,
            nullable: false,
            defaultValue: string.Empty);

        migrationBuilder.AddColumn<string>(
            name: "JMBG",
            table: "AspNetUsers",
            type: "nvarchar(13)",
            maxLength: 13,
            nullable: false,
            defaultValue: string.Empty);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "OIB",
            table: "AspNetUsers");

        migrationBuilder.DropColumn(
            name: "JMBG",
            table: "AspNetUsers");
    }
}
