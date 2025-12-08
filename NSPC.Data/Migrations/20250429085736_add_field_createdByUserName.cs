using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class add_field_createdByUserName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "sm_SocialMediaPost");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModifiedOnDate",
                table: "sm_SocialMediaPost",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<Guid>(
                name: "LastModifiedByUserId",
                table: "sm_SocialMediaPost",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "sm_SocialMediaPost",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedByUserName",
                table: "sm_SocialMediaPost",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "sm_SocialMediaPost");

            migrationBuilder.DropColumn(
                name: "LastModifiedByUserName",
                table: "sm_SocialMediaPost");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModifiedOnDate",
                table: "sm_SocialMediaPost",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastModifiedByUserId",
                table: "sm_SocialMediaPost",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationId",
                table: "sm_SocialMediaPost",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
