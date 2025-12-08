using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NSPC.Data;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class add_table_sm_CommentItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sm_CommentItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CommentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentItems = table.Column<string>(type: "text", nullable: true),
                    CommentItemsAttachments = table.Column<List<jsonb_Attachment>>(type: "jsonb", nullable: true),
                    PostStatusCodeInCommentItems = table.Column<string>(type: "text", nullable: true),
                    TagUserIdsInCommentItems = table.Column<List<string>>(type: "text[]", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_CommentItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_CommentItems_Idm_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Idm_Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_CommentItems_idm_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_CommentItems_sm_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "sm_Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_CommentItems_CommentId",
                table: "sm_CommentItems",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_CommentItems_CreatedByUserId",
                table: "sm_CommentItems",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_CommentItems_TenantId",
                table: "sm_CommentItems",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sm_CommentItems");
        }
    }
}
