using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addprojectIdtblcashbook_transaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "sm_Cashbook_Transaction",
                type: "uuid",
                nullable: true);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "sm_Cashbook_Transaction");       
          
        }
    }
}
