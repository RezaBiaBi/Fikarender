using Microsoft.EntityFrameworkCore.Migrations;

namespace Fikarender.Data.Migrations
{
    public partial class ServiceSortAndWorkSampleEdit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SampleVideoLink",
                schema: "dbo",
                table: "WorkSamples");

            migrationBuilder.AddColumn<byte>(
                name: "Sort",
                schema: "dbo",
                table: "Services",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sort",
                schema: "dbo",
                table: "Services");

            migrationBuilder.AddColumn<string>(
                name: "SampleVideoLink",
                schema: "dbo",
                table: "WorkSamples",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true);
        }
    }
}
