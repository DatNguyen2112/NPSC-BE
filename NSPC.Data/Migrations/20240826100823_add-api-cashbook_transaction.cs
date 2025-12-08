using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addapicashbook_transaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReceiverTen",
                table: "mk_BaoGia",
                newName: "CustomerTaxCode");

            migrationBuilder.RenameColumn(
                name: "ReceiverSoDienThoai",
                table: "mk_BaoGia",
                newName: "CustomerPhoneNumber");

            migrationBuilder.RenameColumn(
                name: "ReceiverMa",
                table: "mk_BaoGia",
                newName: "CustomerName");

            migrationBuilder.RenameColumn(
                name: "ReceiverMST",
                table: "mk_BaoGia",
                newName: "CustomerCode");

            migrationBuilder.RenameColumn(
                name: "ReceiverDiaChi",
                table: "mk_BaoGia",
                newName: "CustomerAddress");

            migrationBuilder.CreateTable(
                name: "sm_Cashbook_Transaction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    PayerGroup = table.Column<string>(type: "text", nullable: true),
                    SupplierId = table.Column<Guid>(type: "uuid", nullable: true),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: true),
                    Purpose = table.Column<string>(type: "text", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: true),
                    PaymentMethod = table.Column<string>(type: "text", nullable: true),
                    ReceiptDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    isStatus = table.Column<bool>(type: "boolean", nullable: false),
                    TransactionType = table.Column<string>(type: "text", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_Cashbook_Transaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_Cashbook_Transaction_mk_DuAn_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "mk_DuAn",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_Cashbook_Transaction_sm_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "sm_Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_Cashbook_Transaction_sm_Supplier_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "sm_Supplier",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_Cashbook_Transaction_CustomerId",
                table: "sm_Cashbook_Transaction",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Cashbook_Transaction_ProjectId",
                table: "sm_Cashbook_Transaction",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Cashbook_Transaction_SupplierId",
                table: "sm_Cashbook_Transaction",
                column: "SupplierId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sm_Cashbook_Transaction");

            migrationBuilder.RenameColumn(
                name: "CustomerTaxCode",
                table: "mk_BaoGia",
                newName: "ReceiverTen");

            migrationBuilder.RenameColumn(
                name: "CustomerPhoneNumber",
                table: "mk_BaoGia",
                newName: "ReceiverSoDienThoai");

            migrationBuilder.RenameColumn(
                name: "CustomerName",
                table: "mk_BaoGia",
                newName: "ReceiverMa");

            migrationBuilder.RenameColumn(
                name: "CustomerCode",
                table: "mk_BaoGia",
                newName: "ReceiverMST");

            migrationBuilder.RenameColumn(
                name: "CustomerAddress",
                table: "mk_BaoGia",
                newName: "ReceiverDiaChi");
        }
    }
}
