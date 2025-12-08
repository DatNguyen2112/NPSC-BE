using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class add_table_sm_SocialMediaPost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sm_SocialMediaPost",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentPostTitle = table.Column<string>(type: "text", nullable: true),
                    ContentPostBody = table.Column<string>(type: "text", nullable: true),
                    PostStatusCode = table.Column<string>(type: "text", nullable: true),
                    TagUserIds = table.Column<List<string>>(type: "text[]", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_SocialMediaPost", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_SocialMediaPost_Idm_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Idm_Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_SocialMediaPost_idm_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_SocialMediaPost_CreatedByUserId",
                table: "sm_SocialMediaPost",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_SocialMediaPost_TenantId",
                table: "sm_SocialMediaPost",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sm_SocialMediaPost");
        }
    }
}
