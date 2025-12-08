using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NSPC.Data.Data.Entity.JsonbEntity;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class Addclassesforhandlinginvoices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sm_EInvoice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    SellerName = table.Column<string>(type: "text", nullable: true),
                    SellerTaxCode = table.Column<string>(type: "text", nullable: true),
                    SellerAddress = table.Column<string>(type: "text", nullable: true),
                    SellerPhoneNumber = table.Column<string>(type: "text", nullable: true),
                    SellerBankAccount = table.Column<string>(type: "text", nullable: true),
                    SellerBankName = table.Column<string>(type: "text", nullable: true),
                    BuyerName = table.Column<string>(type: "text", nullable: true),
                    BuyerTaxCode = table.Column<string>(type: "text", nullable: true),
                    BuyerAddress = table.Column<string>(type: "text", nullable: true),
                    BuyerPhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PaymentMethodName = table.Column<string>(type: "text", nullable: true),
                    BuyerBankAccount = table.Column<string>(type: "text", nullable: true),
                    BuyerBankName = table.Column<string>(type: "text", nullable: true),
                    Note = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    PaymentStatusCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    PaymentStatusName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    PaymentStatusColor = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    TotalBeforeVatAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalVatAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalAmountInWords = table.Column<string>(type: "text", nullable: true),
                    PaidAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    StillInDebtAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    ListOfPaymentHistory = table.Column<List<jsonb_PaymentInvoice>>(type: "jsonb", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_EInvoice", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sm_EInvoiceItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LineNumber = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Unit = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    LineAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    VatPercent = table.Column<decimal>(type: "numeric", nullable: false),
                    VatAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    sm_EInvoiceId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_EInvoiceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_EInvoiceItems_sm_EInvoice_sm_EInvoiceId",
                        column: x => x.sm_EInvoiceId,
                        principalTable: "sm_EInvoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sm_EInvoiceVatAnalytics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Synthetic = table.Column<string>(type: "text", nullable: true),
                    BeforeVatAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    VatAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalPaymentAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    EInvoiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    sm_EInvoiceId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_EInvoiceVatAnalytics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_EInvoiceVatAnalytics_sm_EInvoice_sm_EInvoiceId",
                        column: x => x.sm_EInvoiceId,
                        principalTable: "sm_EInvoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_EInvoiceItems_sm_EInvoiceId",
                table: "sm_EInvoiceItems",
                column: "sm_EInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_EInvoiceVatAnalytics_sm_EInvoiceId",
                table: "sm_EInvoiceVatAnalytics",
                column: "sm_EInvoiceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sm_EInvoiceItems");

            migrationBuilder.DropTable(
                name: "sm_EInvoiceVatAnalytics");

            migrationBuilder.DropTable(
                name: "sm_EInvoice");
        }
    }
}
