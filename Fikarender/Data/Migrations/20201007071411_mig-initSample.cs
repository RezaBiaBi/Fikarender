using Microsoft.EntityFrameworkCore.Migrations;

namespace Fikarender.Data.Migrations
{
    public partial class miginitSample : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Picture",
                schema: "dbo",
                table: "WorkSamples");

            migrationBuilder.AddColumn<string>(
                name: "DocumentFile",
                schema: "dbo",
                table: "WorkSamples",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "SampleType",
                schema: "dbo",
                table: "WorkSamples",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentFile",
                schema: "dbo",
                table: "WorkSamples");

            migrationBuilder.DropColumn(
                name: "SampleType",
                schema: "dbo",
                table: "WorkSamples");

            migrationBuilder.AddColumn<string>(
                name: "Picture",
                schema: "dbo",
                table: "WorkSamples",
                type: "nvarchar(600)",
                maxLength: 600,
                nullable: false,
                defaultValue: "");
        }
    }
}
