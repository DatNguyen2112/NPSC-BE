using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class rename_field_in_sm_LichSuChamSoc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParticipantPersonId",
                table: "sm_LichSuChamSoc");

            migrationBuilder.AddColumn<List<string>>(
                name: "Participants",
                table: "sm_LichSuChamSoc",
                type: "text[]",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Participants",
                table: "sm_LichSuChamSoc");

            migrationBuilder.AddColumn<Guid>(
                name: "ParticipantPersonId",
                table: "sm_LichSuChamSoc",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
