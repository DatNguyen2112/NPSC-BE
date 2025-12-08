using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NSPC.Data;
using NSPC.Data.Entity;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addnewsocialPostandsocialComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sm_SocialPost",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Attachments = table.Column<List<jsonb_Attachment>>(type: "jsonb", nullable: true),
                    Reactions = table.Column<List<jsonb_SocialReaction>>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_SocialPost", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_SocialPost_Idm_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Idm_Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_SocialPost_idm_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_SocialPost_idm_User_LastModifiedByUserId",
                        column: x => x.LastModifiedByUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sm_SocialComment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentCommentId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Attachments = table.Column<List<jsonb_Attachment>>(type: "jsonb", nullable: true),
                    Reactions = table.Column<List<jsonb_SocialReaction>>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_SocialComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_SocialComment_Idm_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Idm_Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_SocialComment_idm_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_SocialComment_idm_User_LastModifiedByUserId",
                        column: x => x.LastModifiedByUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_SocialComment_sm_SocialComment_ParentCommentId",
                        column: x => x.ParentCommentId,
                        principalTable: "sm_SocialComment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_SocialComment_sm_SocialPost_PostId",
                        column: x => x.PostId,
                        principalTable: "sm_SocialPost",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_SocialComment_CreatedByUserId",
                table: "sm_SocialComment",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_SocialComment_LastModifiedByUserId",
                table: "sm_SocialComment",
                column: "LastModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_SocialComment_ParentCommentId",
                table: "sm_SocialComment",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_SocialComment_PostId",
                table: "sm_SocialComment",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_SocialComment_TenantId",
                table: "sm_SocialComment",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_SocialPost_CreatedByUserId",
                table: "sm_SocialPost",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_SocialPost_LastModifiedByUserId",
                table: "sm_SocialPost",
                column: "LastModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_SocialPost_TenantId",
                table: "sm_SocialPost",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sm_SocialComment");

            migrationBuilder.DropTable(
                name: "sm_SocialPost");
        }
    }
}
