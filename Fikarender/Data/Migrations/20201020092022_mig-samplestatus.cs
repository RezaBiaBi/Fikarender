using Microsoft.EntityFrameworkCore.Migrations;

namespace Fikarender.Data.Migrations
{
    public partial class migsamplestatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                schema: "dbo",
                table: "WorkSamples",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldMaxLength: 200);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "Status",
                schema: "dbo",
                table: "WorkSamples",
                type: "tinyint",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(bool));
        }
    }
}
