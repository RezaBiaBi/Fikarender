using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Fikarender.Data.Migrations
{
    public partial class initialDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                newName: "AspNetUserTokens",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                newName: "AspNetUsers",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                newName: "AspNetUserRoles",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                newName: "AspNetUserLogins",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                newName: "AspNetUserClaims",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                newName: "AspNetRoles",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                newName: "AspNetRoleClaims",
                newSchema: "dbo");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dbo",
                table: "AspNetUserTokens",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                schema: "dbo",
                table: "AspNetUserTokens",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                schema: "dbo",
                table: "AspNetUserLogins",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                schema: "dbo",
                table: "AspNetUserLogins",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.CreateTable(
                name: "AdminTheme",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(maxLength: 450, nullable: false),
                    Theme = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminTheme", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Assists",
                schema: "dbo",
                columns: table => new
                {
                    AssistId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(maxLength: 200, nullable: false),
                    Location = table.Column<string>(maxLength: 50, nullable: false),
                    SocialId = table.Column<string>(maxLength: 400, nullable: false),
                    PhoneNumber = table.Column<string>(maxLength: 11, nullable: false),
                    Graphic = table.Column<bool>(nullable: false),
                    MotionGraphic = table.Column<bool>(nullable: false),
                    StopMotion = table.Column<bool>(nullable: false),
                    Animate = table.Column<bool>(nullable: false),
                    Logo = table.Column<bool>(nullable: false),
                    Ui = table.Column<bool>(nullable: false),
                    Ux = table.Column<bool>(nullable: false),
                    Modeling3D = table.Column<bool>(nullable: false),
                    EditingVideo = table.Column<bool>(nullable: false),
                    Photography = table.Column<bool>(nullable: false),
                    Filming = table.Column<bool>(nullable: false),
                    ContentAuthor = table.Column<bool>(nullable: false),
                    DigitalIllustrator = table.Column<bool>(nullable: false),
                    OtherSpecialty = table.Column<string>(nullable: true),
                    CvFileName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assists", x => x.AssistId);
                });

            migrationBuilder.CreateTable(
                name: "Blog",
                schema: "dbo",
                columns: table => new
                {
                    BlogId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(maxLength: 256, nullable: false),
                    MetaTitle = table.Column<string>(maxLength: 256, nullable: true),
                    Description = table.Column<string>(maxLength: 512, nullable: true),
                    MetaDescription = table.Column<string>(maxLength: 512, nullable: false),
                    Summery = table.Column<string>(maxLength: 512, nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: false),
                    Content = table.Column<string>(nullable: false),
                    DeskImage = table.Column<string>(maxLength: 50, nullable: true),
                    MobileImage = table.Column<string>(maxLength: 50, nullable: true),
                    ViewCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blog", x => x.BlogId);
                });

            migrationBuilder.CreateTable(
                name: "Faqs",
                schema: "dbo",
                columns: table => new
                {
                    FaqId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Question = table.Column<string>(nullable: false),
                    Answer = table.Column<string>(nullable: false),
                    Sort = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faqs", x => x.FaqId);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                schema: "dbo",
                columns: table => new
                {
                    ServiceId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceTitle = table.Column<string>(maxLength: 100, nullable: false),
                    ServiceMetaTitle = table.Column<string>(maxLength: 200, nullable: true),
                    ServiceMetaDesc = table.Column<string>(maxLength: 600, nullable: true),
                    ParentId = table.Column<int>(nullable: true),
                    ServicePicture = table.Column<string>(maxLength: 600, nullable: false),
                    Content = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.ServiceId);
                });

            migrationBuilder.CreateTable(
                name: "ShortLink",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShortKey = table.Column<string>(maxLength: 5, nullable: false),
                    Type = table.Column<byte>(nullable: false),
                    ItemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShortLink", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tag",
                schema: "dbo",
                columns: table => new
                {
                    TagId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    UniqueName = table.Column<string>(maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.TagId);
                });

            migrationBuilder.CreateTable(
                name: "Team",
                schema: "dbo",
                columns: table => new
                {
                    TeamId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(maxLength: 300, nullable: false),
                    Email = table.Column<string>(maxLength: 256, nullable: false),
                    PhoneNumber = table.Column<string>(maxLength: 11, nullable: false),
                    Degree = table.Column<string>(maxLength: 200, nullable: false),
                    Avatar = table.Column<string>(maxLength: 50, nullable: true),
                    Description = table.Column<string>(maxLength: 256, nullable: false),
                    Sort = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Team", x => x.TeamId);
                });

            migrationBuilder.CreateTable(
                name: "UploadedFiles",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Type = table.Column<string>(maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadedFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsersFeedbacks",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(maxLength: 256, nullable: false),
                    UserRole = table.Column<string>(maxLength: 256, nullable: false),
                    Description = table.Column<string>(maxLength: 256, nullable: false),
                    Sort = table.Column<byte>(nullable: false),
                    Picture = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersFeedbacks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkSamples",
                schema: "dbo",
                columns: table => new
                {
                    WorkSampleId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(maxLength: 200, nullable: false),
                    MetaTitle = table.Column<string>(maxLength: 200, nullable: false),
                    ServiceId = table.Column<int>(nullable: false),
                    IsShow = table.Column<bool>(nullable: false),
                    Status = table.Column<byte>(maxLength: 200, nullable: false),
                    Description = table.Column<string>(maxLength: 600, nullable: false),
                    Picture = table.Column<string>(maxLength: 600, nullable: false),
                    LongContent = table.Column<string>(maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkSamples", x => x.WorkSampleId);
                    table.ForeignKey(
                        name: "FK_WorkSamples_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "dbo",
                        principalTable: "Services",
                        principalColumn: "ServiceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlogTag",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlogId = table.Column<int>(nullable: false),
                    TagId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogTag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlogTag_Blog_BlogId",
                        column: x => x.BlogId,
                        principalSchema: "dbo",
                        principalTable: "Blog",
                        principalColumn: "BlogId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlogTag_Tag_TagId",
                        column: x => x.TagId,
                        principalSchema: "dbo",
                        principalTable: "Tag",
                        principalColumn: "TagId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FaqTags",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FaqId = table.Column<int>(nullable: false),
                    TagId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaqTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FaqTags_Faqs_FaqId",
                        column: x => x.FaqId,
                        principalSchema: "dbo",
                        principalTable: "Faqs",
                        principalColumn: "FaqId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FaqTags_Tag_TagId",
                        column: x => x.TagId,
                        principalSchema: "dbo",
                        principalTable: "Tag",
                        principalColumn: "TagId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogTag_BlogId",
                schema: "dbo",
                table: "BlogTag",
                column: "BlogId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogTag_TagId",
                schema: "dbo",
                table: "BlogTag",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_FaqTags_FaqId",
                schema: "dbo",
                table: "FaqTags",
                column: "FaqId");

            migrationBuilder.CreateIndex(
                name: "IX_FaqTags_TagId",
                schema: "dbo",
                table: "FaqTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkSamples_ServiceId",
                schema: "dbo",
                table: "WorkSamples",
                column: "ServiceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminTheme",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Assists",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BlogTag",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "FaqTags",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ShortLink",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Team",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UploadedFiles",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UsersFeedbacks",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "WorkSamples",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Blog",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Faqs",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Tag",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Services",
                schema: "dbo");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                schema: "dbo",
                newName: "AspNetUserTokens");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                schema: "dbo",
                newName: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                schema: "dbo",
                newName: "AspNetUserRoles");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                schema: "dbo",
                newName: "AspNetUserLogins");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                schema: "dbo",
                newName: "AspNetUserClaims");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                schema: "dbo",
                newName: "AspNetRoles");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                schema: "dbo",
                newName: "AspNetRoleClaims");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
