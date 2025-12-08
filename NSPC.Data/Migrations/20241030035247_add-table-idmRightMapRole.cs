using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addtableidmRightMapRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IdmRightMapRole",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleCode = table.Column<string>(type: "text", nullable: true),
                    RightCode = table.Column<string>(type: "text", nullable: true),
                    RightId = table.Column<Guid>(type: "uuid", nullable: true),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdmRightMapRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdmRightMapRole_idm_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "idm_Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IdmRightMapRole_IdmRight_RightId",
                        column: x => x.RightId,
                        principalTable: "IdmRight",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IdmRightMapRole_RightId",
                table: "IdmRightMapRole",
                column: "RightId");

            migrationBuilder.CreateIndex(
                name: "IX_IdmRightMapRole_RoleId",
                table: "IdmRightMapRole",
                column: "RoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdmRightMapRole");
        }
    }
}
