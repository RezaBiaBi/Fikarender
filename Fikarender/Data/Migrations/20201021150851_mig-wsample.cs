using Microsoft.EntityFrameworkCore.Migrations;

namespace Fikarender.Data.Migrations
{
    public partial class migwsample : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SampleType",
                schema: "dbo",
                table: "WorkSamples");

            migrationBuilder.AlterColumn<string>(
                name: "LongContent",
                schema: "dbo",
                table: "WorkSamples",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1024)",
                oldMaxLength: 1024);

            migrationBuilder.AddColumn<string>(
                name: "SampleVideoLink",
                schema: "dbo",
                table: "WorkSamples",
                maxLength: 1024,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SampleVideoLink",
                schema: "dbo",
                table: "WorkSamples");

            migrationBuilder.AlterColumn<string>(
                name: "LongContent",
                schema: "dbo",
                table: "WorkSamples",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<byte>(
                name: "SampleType",
                schema: "dbo",
                table: "WorkSamples",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
