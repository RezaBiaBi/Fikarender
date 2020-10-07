using Microsoft.EntityFrameworkCore.Migrations;

namespace Fikarender.Data.Migrations
{
    public partial class ServiceParentId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                schema: "dbo",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Services_ParentId",
                schema: "dbo",
                table: "Services",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Services_ParentId",
                schema: "dbo",
                table: "Services",
                column: "ParentId",
                principalSchema: "dbo",
                principalTable: "Services",
                principalColumn: "ServiceId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_Services_ParentId",
                schema: "dbo",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_ParentId",
                schema: "dbo",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                schema: "dbo",
                table: "AspNetUsers");
        }
    }
}
