using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "bsd_Key_Value",
                columns: table => new
                {
                    Key = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bsd_Key_Value", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "bsd_Navigation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    UrlRewrite = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    IdPath = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    Path = table.Column<string>(type: "character varying(900)", maxLength: 900, nullable: false),
                    SubUrl = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    IconClass = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: true),
                    HasChild = table.Column<bool>(type: "boolean", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: true),
                    QueryParams = table.Column<string>(type: "text", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bsd_Navigation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_bsd_Navigation_bsd_Navigation_ParentId",
                        column: x => x.ParentId,
                        principalTable: "bsd_Navigation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "bsd_Parameter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    Value = table.Column<string>(type: "text", nullable: true),
                    IsSystem = table.Column<bool>(type: "boolean", nullable: false),
                    GroupCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bsd_Parameter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "cata_Commune",
                columns: table => new
                {
                    CommuneCode = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CommuneName = table.Column<string>(type: "text", nullable: true),
                    DistrictCode = table.Column<int>(type: "integer", nullable: false),
                    WardId_VP = table.Column<int>(type: "integer", nullable: false),
                    WardName_VP = table.Column<string>(type: "text", nullable: true),
                    DistrictId_VP = table.Column<int>(type: "integer", nullable: false),
                    VNPAsciiName = table.Column<string>(type: "text", nullable: true),
                    VTPAsciiName = table.Column<string>(type: "text", nullable: true),
                    OldVTPName = table.Column<string>(type: "text", nullable: true),
                    VnpostSyncStatus = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cata_Commune", x => x.CommuneCode);
                });

            migrationBuilder.CreateTable(
                name: "cata_District",
                columns: table => new
                {
                    DistrictCode = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DistrictName = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ProvinceCode = table.Column<int>(type: "integer", nullable: false),
                    DistrictShippingType = table.Column<int>(type: "integer", nullable: false),
                    DistrictId_VP = table.Column<int>(type: "integer", nullable: false),
                    DistrictValue_VP = table.Column<string>(type: "text", nullable: true),
                    DistrictName_VP = table.Column<string>(type: "text", nullable: true),
                    ProvinceId_VP = table.Column<int>(type: "integer", nullable: false),
                    DelayShipFromDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    StopShipFromDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DelayShipToDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    StopShipToDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    VnpostSyncStatus = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cata_District", x => x.DistrictCode);
                });

            migrationBuilder.CreateTable(
                name: "cata_Province",
                columns: table => new
                {
                    ProvinceCode = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProvinceName = table.Column<string>(type: "text", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    ProvinceShippingType = table.Column<int>(type: "integer", nullable: false),
                    ProvinceId_VP = table.Column<int>(type: "integer", nullable: false),
                    ProvinceCode_VP = table.Column<string>(type: "text", nullable: true),
                    ProvinceName_VP = table.Column<string>(type: "text", nullable: true),
                    ServiceType_VP = table.Column<int>(type: "integer", nullable: false),
                    TotalDistrictShipDelay = table.Column<int>(type: "integer", nullable: false),
                    TotalDistrictShipStop = table.Column<int>(type: "integer", nullable: false),
                    DelayShipFromDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    StopShipFromDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DelayShipToDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    StopShipToDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cata_Province", x => x.ProvinceCode);
                });

            migrationBuilder.CreateTable(
                name: "erp_Attachment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DocType = table.Column<string>(type: "text", nullable: true),
                    DocTypeName = table.Column<string>(type: "text", nullable: true),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    EntityType = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    FilePath = table.Column<string>(type: "text", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FileType = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    OriginalFileName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    StatusCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    UpdateFrequency = table.Column<string>(type: "text", nullable: true),
                    Language = table.Column<string>(type: "text", nullable: true),
                    CopyRight = table.Column<string>(type: "text", nullable: true),
                    IsPrivate = table.Column<bool>(type: "boolean", nullable: false),
                    License = table.Column<string>(type: "text", nullable: true),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_erp_Attachment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "idm_Role",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    IsSystem = table.Column<bool>(type: "boolean", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_idm_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "idm_User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    CountryCode = table.Column<string>(type: "text", nullable: true),
                    Currency = table.Column<string>(type: "text", nullable: true),
                    Language = table.Column<string>(type: "text", nullable: true),
                    Gender = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    AvatarUrl = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    Password = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    PasswordSalt = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    Birthdate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastActivityDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PlainTextPwd = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    IsLockedOut = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ActiveDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    UpdateLog = table.Column<string>(type: "text", nullable: true),
                    FacebookUserId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    GoogleUserId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ResetPasswordToken = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    EmailVerifyToken = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    IsEmailVerified = table.Column<bool>(type: "boolean", nullable: false),
                    RoleListCode = table.Column<List<string>>(type: "text[]", nullable: true),
                    BankAccountNo = table.Column<string>(type: "text", nullable: true),
                    BankName = table.Column<string>(type: "text", nullable: true),
                    BankUsername = table.Column<string>(type: "text", nullable: true),
                    StatusCode = table.Column<string>(type: "text", nullable: true),
                    DeviceToken = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_idm_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "mk_PhongBan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaPhongBan = table.Column<string>(type: "text", nullable: false),
                    TenPhongBan = table.Column<string>(type: "text", nullable: false),
                    GhiChu = table.Column<string>(type: "text", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mk_PhongBan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sm_ActiviyHisroty",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityType = table.Column<string>(type: "text", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_ActiviyHisroty", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sm_CodeType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Code = table.Column<string>(type: "text", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    TranslationCount = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_CodeType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sm_Email_Subscribe",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    SubscribeDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UnsubscribeDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    TotalEmailSentCount = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_Email_Subscribe", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sm_Email_Template",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_Email_Template", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sm_Email_Verification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Verified = table.Column<bool>(type: "boolean", nullable: false),
                    ValidDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    VerifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    VerifyToken = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_Email_Verification", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sm_Notification_Template",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_Notification_Template", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "bsd_Navigation_Map_Role",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NavigationId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromSubNavigation = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bsd_Navigation_Map_Role", x => x.Id);
                    table.ForeignKey(
                        name: "FK_bsd_Navigation_Map_Role_bsd_Navigation_NavigationId",
                        column: x => x.NavigationId,
                        principalTable: "bsd_Navigation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sm_KhachHang",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Ma = table.Column<string>(type: "text", nullable: true),
                    Ten = table.Column<string>(type: "text", nullable: true),
                    GhiChu = table.Column<string>(type: "text", nullable: true),
                    SoDienThoai = table.Column<string>(type: "text", nullable: true),
                    DiaChi = table.Column<string>(type: "text", nullable: true),
                    Birthdate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LinkFacebook = table.Column<string>(type: "text", nullable: true),
                    LinkTiktok = table.Column<string>(type: "text", nullable: true),
                    LinkTelegram = table.Column<string>(type: "text", nullable: true),
                    LoaiKhachHang = table.Column<List<string>>(type: "text[]", nullable: true),
                    TrangThai = table.Column<string>(type: "text", nullable: true),
                    LastCareOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TotalCareTimes = table.Column<int>(type: "integer", nullable: false),
                    NhuCauBanDau = table.Column<string>(type: "text", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_KhachHang", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_KhachHang_idm_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sm_Notification",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReceiverUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsReceiverRead = table.Column<bool>(type: "boolean", nullable: false),
                    IsReceiverSeen = table.Column<bool>(type: "boolean", nullable: false),
                    ReceiverReadOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    JsonData = table.Column<string>(type: "text", nullable: true),
                    JsonDataType = table.Column<string>(type: "text", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_Notification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_Notification_idm_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_Notification_idm_User_ReceiverUserId",
                        column: x => x.ReceiverUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sm_CodeType_Translation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Language = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    CodeTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_CodeType_Translation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_CodeType_Translation_sm_CodeType_CodeTypeId",
                        column: x => x.CodeTypeId,
                        principalTable: "sm_CodeType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sm_Email_Template_Translations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmailTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    TitleTemplate = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    BodyTemplate = table.Column<string>(type: "text", nullable: true),
                    Language = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_Email_Template_Translations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_Email_Template_Translations_sm_Email_Template_EmailTempl~",
                        column: x => x.EmailTemplateId,
                        principalTable: "sm_Email_Template",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sm_Notification_Template_Translation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NotificationTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    TitleTemplate = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    BodyPlainTextTemplate = table.Column<string>(type: "text", nullable: true),
                    BodyHtmlTemplate = table.Column<string>(type: "text", nullable: true),
                    Language = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_Notification_Template_Translation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_Notification_Template_Translation_sm_Notification_Templa~",
                        column: x => x.NotificationTemplateId,
                        principalTable: "sm_Notification_Template",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sm_LichSuChamSoc",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GhiChu = table.Column<string>(type: "text", nullable: true),
                    DanhGia = table.Column<int>(type: "integer", nullable: false),
                    KhachHangId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_LichSuChamSoc", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_LichSuChamSoc_idm_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_LichSuChamSoc_sm_KhachHang_KhachHangId",
                        column: x => x.KhachHangId,
                        principalTable: "sm_KhachHang",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_bsd_Navigation_ParentId",
                table: "bsd_Navigation",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_bsd_Navigation_Map_Role_NavigationId",
                table: "bsd_Navigation_Map_Role",
                column: "NavigationId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_CodeType_Translation_CodeTypeId",
                table: "sm_CodeType_Translation",
                column: "CodeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Email_Template_Translations_EmailTemplateId",
                table: "sm_Email_Template_Translations",
                column: "EmailTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_KhachHang_CreatedByUserId",
                table: "sm_KhachHang",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_LichSuChamSoc_CreatedByUserId",
                table: "sm_LichSuChamSoc",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_LichSuChamSoc_KhachHangId",
                table: "sm_LichSuChamSoc",
                column: "KhachHangId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Notification_CreatedByUserId",
                table: "sm_Notification",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Notification_ReceiverUserId",
                table: "sm_Notification",
                column: "ReceiverUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Notification_Template_Translation_NotificationTemplateId",
                table: "sm_Notification_Template_Translation",
                column: "NotificationTemplateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bsd_Key_Value");

            migrationBuilder.DropTable(
                name: "bsd_Navigation_Map_Role");

            migrationBuilder.DropTable(
                name: "bsd_Parameter");

            migrationBuilder.DropTable(
                name: "cata_Commune");

            migrationBuilder.DropTable(
                name: "cata_District");

            migrationBuilder.DropTable(
                name: "cata_Province");

            migrationBuilder.DropTable(
                name: "erp_Attachment");

            migrationBuilder.DropTable(
                name: "idm_Role");

            migrationBuilder.DropTable(
                name: "mk_PhongBan");

            migrationBuilder.DropTable(
                name: "sm_ActiviyHisroty");

            migrationBuilder.DropTable(
                name: "sm_CodeType_Translation");

            migrationBuilder.DropTable(
                name: "sm_Email_Subscribe");

            migrationBuilder.DropTable(
                name: "sm_Email_Template_Translations");

            migrationBuilder.DropTable(
                name: "sm_Email_Verification");

            migrationBuilder.DropTable(
                name: "sm_LichSuChamSoc");

            migrationBuilder.DropTable(
                name: "sm_Notification");

            migrationBuilder.DropTable(
                name: "sm_Notification_Template_Translation");

            migrationBuilder.DropTable(
                name: "bsd_Navigation");

            migrationBuilder.DropTable(
                name: "sm_CodeType");

            migrationBuilder.DropTable(
                name: "sm_Email_Template");

            migrationBuilder.DropTable(
                name: "sm_KhachHang");

            migrationBuilder.DropTable(
                name: "sm_Notification_Template");

            migrationBuilder.DropTable(
                name: "idm_User");
        }
    }
}
