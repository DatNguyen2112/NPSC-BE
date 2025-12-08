using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class editassetv2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_Asset_mk_PhongBan_PlaceOfUseId",
                table: "sm_Asset");

            migrationBuilder.DropColumn(
                name: "CertificateDate",
                table: "sm_Asset");

            migrationBuilder.DropColumn(
                name: "CertificateNumber",
                table: "sm_Asset");

            migrationBuilder.DropColumn(
                name: "Depreciation",
                table: "sm_Asset");

            migrationBuilder.DropColumn(
                name: "GroupCode",
                table: "sm_Asset");

            migrationBuilder.DropColumn(
                name: "GroupName",
                table: "sm_Asset");

            migrationBuilder.DropColumn(
                name: "Origin",
                table: "sm_Asset");

            migrationBuilder.DropColumn(
                name: "TypeCode",
                table: "sm_Asset");

            migrationBuilder.DropColumn(
                name: "TypeName",
                table: "sm_Asset");

            migrationBuilder.RenameColumn(
                name: "StartOfUseDate",
                table: "sm_Asset",
                newName: "DepreciationStartDate");

            migrationBuilder.RenameColumn(
                name: "PlaceOfUseId",
                table: "sm_Asset",
                newName: "AssetTypeId");

            migrationBuilder.RenameColumn(
                name: "MaintenanceCycle",
                table: "sm_Asset",
                newName: "RemainingValue");

            migrationBuilder.RenameIndex(
                name: "IX_sm_Asset_PlaceOfUseId",
                table: "sm_Asset",
                newName: "IX_sm_Asset_AssetTypeId");

            migrationBuilder.AlterColumn<string>(
                name: "Serial",
                table: "sm_Asset",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "PurchasedDate",
                table: "sm_Asset",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddColumn<decimal>(
                name: "AccumulatedDepreciation",
                table: "sm_Asset",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssetLocationId",
                table: "sm_Asset",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "DepreciationTime",
                table: "sm_Asset",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DepreciationValue",
                table: "sm_Asset",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MeasureUnitId",
                table: "sm_Asset",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_Asset_AssetLocationId",
                table: "sm_Asset",
                column: "AssetLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Asset_MeasureUnitId",
                table: "sm_Asset",
                column: "MeasureUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Asset_sm_AssetLocation_AssetLocationId",
                table: "sm_Asset",
                column: "AssetLocationId",
                principalTable: "sm_AssetLocation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Asset_sm_AssetType_AssetTypeId",
                table: "sm_Asset",
                column: "AssetTypeId",
                principalTable: "sm_AssetType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Asset_sm_MeasureUnit_MeasureUnitId",
                table: "sm_Asset",
                column: "MeasureUnitId",
                principalTable: "sm_MeasureUnit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_Asset_sm_AssetLocation_AssetLocationId",
                table: "sm_Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_Asset_sm_AssetType_AssetTypeId",
                table: "sm_Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_Asset_sm_MeasureUnit_MeasureUnitId",
                table: "sm_Asset");

            migrationBuilder.DropIndex(
                name: "IX_sm_Asset_AssetLocationId",
                table: "sm_Asset");

            migrationBuilder.DropIndex(
                name: "IX_sm_Asset_MeasureUnitId",
                table: "sm_Asset");

            migrationBuilder.DropColumn(
                name: "AccumulatedDepreciation",
                table: "sm_Asset");

            migrationBuilder.DropColumn(
                name: "AssetLocationId",
                table: "sm_Asset");

            migrationBuilder.DropColumn(
                name: "DepreciationTime",
                table: "sm_Asset");

            migrationBuilder.DropColumn(
                name: "DepreciationValue",
                table: "sm_Asset");

            migrationBuilder.DropColumn(
                name: "MeasureUnitId",
                table: "sm_Asset");

            migrationBuilder.RenameColumn(
                name: "RemainingValue",
                table: "sm_Asset",
                newName: "MaintenanceCycle");

            migrationBuilder.RenameColumn(
                name: "DepreciationStartDate",
                table: "sm_Asset",
                newName: "StartOfUseDate");

            migrationBuilder.RenameColumn(
                name: "AssetTypeId",
                table: "sm_Asset",
                newName: "PlaceOfUseId");

            migrationBuilder.RenameIndex(
                name: "IX_sm_Asset_AssetTypeId",
                table: "sm_Asset",
                newName: "IX_sm_Asset_PlaceOfUseId");

            migrationBuilder.AlterColumn<string>(
                name: "Serial",
                table: "sm_Asset",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "PurchasedDate",
                table: "sm_Asset",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CertificateDate",
                table: "sm_Asset",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CertificateNumber",
                table: "sm_Asset",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Depreciation",
                table: "sm_Asset",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "GroupCode",
                table: "sm_Asset",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GroupName",
                table: "sm_Asset",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Origin",
                table: "sm_Asset",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TypeCode",
                table: "sm_Asset",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TypeName",
                table: "sm_Asset",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Asset_mk_PhongBan_PlaceOfUseId",
                table: "sm_Asset",
                column: "PlaceOfUseId",
                principalTable: "mk_PhongBan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
