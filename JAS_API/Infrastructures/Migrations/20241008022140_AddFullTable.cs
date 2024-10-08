using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class AddFullTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AddressToShip_Accounts_AccountId",
                table: "AddressToShip");

            migrationBuilder.DropForeignKey(
                name: "FK_AddressToShip_Ward_WardId",
                table: "AddressToShip");

            migrationBuilder.DropForeignKey(
                name: "FK_BidLimits_Accounts_AccountId",
                table: "BidLimits");

            migrationBuilder.DropForeignKey(
                name: "FK_District_Province_ProvinceId",
                table: "District");

            migrationBuilder.DropForeignKey(
                name: "FK_ImageJewelry_Jewelries_JewelryId",
                table: "ImageJewelry");

            migrationBuilder.DropForeignKey(
                name: "FK_Jewelries_Genders_GenderId",
                table: "Jewelries");

            migrationBuilder.DropForeignKey(
                name: "FK_ValuationDocuments_ValuationDocumentTypes_ValuationDocument~",
                table: "ValuationDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_Valuations_Accounts_SellerId",
                table: "Valuations");

            migrationBuilder.DropForeignKey(
                name: "FK_Valuations_Accounts_StaffId",
                table: "Valuations");

            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_Accounts_AccountId",
                table: "Wallets");

            migrationBuilder.DropForeignKey(
                name: "FK_Ward_District_DistrictId",
                table: "Ward");

            migrationBuilder.DropTable(
                name: "Genders");

            migrationBuilder.DropTable(
                name: "Proofs");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "ValuationDocumentTypes");

            migrationBuilder.DropTable(
                name: "ProofTypes");

            migrationBuilder.DropTable(
                name: "TransactionTypes");

            migrationBuilder.DropIndex(
                name: "IX_ValuationDocuments_ValuationDocumentTypeId",
                table: "ValuationDocuments");

            migrationBuilder.DropIndex(
                name: "IX_Jewelries_GenderId",
                table: "Jewelries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ward",
                table: "Ward");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Province",
                table: "Province");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ImageJewelry",
                table: "ImageJewelry");

            migrationBuilder.DropPrimaryKey(
                name: "PK_District",
                table: "District");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AddressToShip",
                table: "AddressToShip");

            migrationBuilder.DropColumn(
                name: "DeliveryDate",
                table: "Valuations");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Valuations");

            migrationBuilder.DropColumn(
                name: "ValuationDocumentTypeId",
                table: "ValuationDocuments");

            migrationBuilder.DropColumn(
                name: "GenderId",
                table: "Jewelries");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "CitizenIdentificationCard",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "IDExpirationDate",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "IDIssuanceDate",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "VNPayAccount",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "VNPayAccountName",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "VNPayBankCode",
                table: "Accounts");

            migrationBuilder.RenameTable(
                name: "Ward",
                newName: "Wards");

            migrationBuilder.RenameTable(
                name: "Province",
                newName: "Provinces");

            migrationBuilder.RenameTable(
                name: "ImageJewelry",
                newName: "ImageJewelrys");

            migrationBuilder.RenameTable(
                name: "District",
                newName: "Districts");

            migrationBuilder.RenameTable(
                name: "AddressToShip",
                newName: "AddressToShips");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "Wallets",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Wallets_AccountId",
                table: "Wallets",
                newName: "IX_Wallets_CustomerId");

            migrationBuilder.RenameColumn(
                name: "DesiredPrice",
                table: "Valuations",
                newName: "EstimatePriceMin");

            migrationBuilder.RenameColumn(
                name: "FileDocument",
                table: "ValuationDocuments",
                newName: "ValuationDocumentType");

            migrationBuilder.RenameColumn(
                name: "ReserverPrice",
                table: "Jewelries",
                newName: "StartingPrice");

            migrationBuilder.RenameColumn(
                name: "FinalPrice",
                table: "Jewelries",
                newName: "EstimatePriceMin");

            migrationBuilder.RenameColumn(
                name: "FileVideo",
                table: "Jewelries",
                newName: "VideoLink");

            migrationBuilder.RenameColumn(
                name: "Condition",
                table: "Jewelries",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "BidLimits",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_BidLimits_AccountId",
                table: "BidLimits",
                newName: "IX_BidLimits_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Ward_DistrictId",
                table: "Wards",
                newName: "IX_Wards_DistrictId");

            migrationBuilder.RenameColumn(
                name: "File",
                table: "ImageJewelrys",
                newName: "Title");

            migrationBuilder.RenameIndex(
                name: "IX_ImageJewelry_JewelryId",
                table: "ImageJewelrys",
                newName: "IX_ImageJewelrys_JewelryId");

            migrationBuilder.RenameIndex(
                name: "IX_District_ProvinceId",
                table: "Districts",
                newName: "IX_Districts_ProvinceId");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "AddressToShips",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_AddressToShip_WardId",
                table: "AddressToShips",
                newName: "IX_AddressToShips_WardId");

            migrationBuilder.RenameIndex(
                name: "IX_AddressToShip_AccountId",
                table: "AddressToShips",
                newName: "IX_AddressToShips_CustomerId");

            migrationBuilder.AddColumn<string>(
                name: "CancelReason",
                table: "Valuations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "EstimatePriceMax",
                table: "Valuations",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentLink",
                table: "ValuationDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SignatureCode",
                table: "ValuationDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BidForm",
                table: "Jewelries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "EstimatePriceMax",
                table: "Jewelries",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ForGender",
                table: "Jewelries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Time_Bidding",
                table: "Jewelries",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefaultImage",
                table: "ImageValuations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "ImageValuations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageLink",
                table: "ImageJewelrys",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailImage",
                table: "ImageJewelrys",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Wards",
                table: "Wards",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Provinces",
                table: "Provinces",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImageJewelrys",
                table: "ImageJewelrys",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Districts",
                table: "Districts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AddressToShips",
                table: "AddressToShips",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Auctions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EndTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ActualEndTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Location = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auctions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BidIncrements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    From = table.Column<float>(type: "real", nullable: true),
                    To = table.Column<float>(type: "real", nullable: true),
                    PricePerStep = table.Column<float>(type: "real", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BidIncrements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    ProfilePicture = table.Column<string>(type: "text", nullable: true),
                    Gender = table.Column<string>(type: "text", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    CitizenIdentificationCard = table.Column<string>(type: "text", nullable: true),
                    IDIssuanceDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IDExpirationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AccountId = table.Column<int>(type: "integer", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customers_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FeeShips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    From = table.Column<float>(type: "real", nullable: true),
                    To = table.Column<float>(type: "real", nullable: true),
                    Free = table.Column<float>(type: "real", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeeShips", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FloorFeePersents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    From = table.Column<float>(type: "real", nullable: true),
                    To = table.Column<float>(type: "real", nullable: true),
                    Percent = table.Column<float>(type: "real", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FloorFeePersents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HistoryValuations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StatusName = table.Column<string>(type: "text", nullable: true),
                    ValuationId = table.Column<int>(type: "integer", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoryValuations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoryValuations_Valuations_ValuationId",
                        column: x => x.ValuationId,
                        principalTable: "Valuations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MainDiamonds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Color = table.Column<string>(type: "text", nullable: true),
                    Cut = table.Column<string>(type: "text", nullable: true),
                    Clarity = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: true),
                    SettingType = table.Column<string>(type: "text", nullable: true),
                    Dimension = table.Column<float>(type: "real", nullable: true),
                    Shape = table.Column<string>(type: "text", nullable: true),
                    Certificate = table.Column<string>(type: "text", nullable: true),
                    Fluorescence = table.Column<string>(type: "text", nullable: true),
                    LengthWidthRatio = table.Column<float>(type: "real", nullable: true),
                    JewelryId = table.Column<int>(type: "integer", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainDiamonds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MainDiamonds_Jewelries_JewelryId",
                        column: x => x.JewelryId,
                        principalTable: "Jewelries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MainShaphies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Color = table.Column<string>(type: "text", nullable: true),
                    Carat = table.Column<float>(type: "real", nullable: true),
                    EnhancementType = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: true),
                    SettingType = table.Column<string>(type: "text", nullable: true),
                    Dimension = table.Column<float>(type: "real", nullable: true),
                    JewelryId = table.Column<int>(type: "integer", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainShaphies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MainShaphies_Jewelries_JewelryId",
                        column: x => x.JewelryId,
                        principalTable: "Jewelries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Is_Read = table.Column<bool>(type: "boolean", nullable: true),
                    NotifiableId = table.Column<int>(type: "integer", nullable: true),
                    Notifi_Type = table.Column<string>(type: "text", nullable: true),
                    AccountId = table.Column<int>(type: "integer", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequestWithdraws",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Amount = table.Column<float>(type: "real", nullable: true),
                    WalletId = table.Column<int>(type: "integer", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestWithdraws", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestWithdraws_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SecondaryDiamonds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Color = table.Column<string>(type: "text", nullable: true),
                    Cut = table.Column<string>(type: "text", nullable: true),
                    Clarity = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: true),
                    SettingType = table.Column<string>(type: "text", nullable: true),
                    Dimension = table.Column<float>(type: "real", nullable: true),
                    Shape = table.Column<string>(type: "text", nullable: true),
                    Certificate = table.Column<string>(type: "text", nullable: true),
                    Fluorescence = table.Column<string>(type: "text", nullable: true),
                    LengthWidthRatio = table.Column<float>(type: "real", nullable: true),
                    JewelryId = table.Column<int>(type: "integer", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecondaryDiamonds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SecondaryDiamonds_Jewelries_JewelryId",
                        column: x => x.JewelryId,
                        principalTable: "Jewelries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SecondaryShaphies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Color = table.Column<string>(type: "text", nullable: true),
                    Carat = table.Column<float>(type: "real", nullable: true),
                    EnhancementType = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: true),
                    SettingType = table.Column<string>(type: "text", nullable: true),
                    Dimension = table.Column<float>(type: "real", nullable: true),
                    JewelryId = table.Column<int>(type: "integer", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecondaryShaphies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SecondaryShaphies_Jewelries_JewelryId",
                        column: x => x.JewelryId,
                        principalTable: "Jewelries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Staffs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    ProfilePicture = table.Column<string>(type: "text", nullable: true),
                    Gender = table.Column<string>(type: "text", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AccountId = table.Column<int>(type: "integer", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staffs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Staffs_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WalletTransaction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DocNo = table.Column<int>(type: "integer", nullable: true),
                    Amount = table.Column<float>(type: "real", nullable: true),
                    TransactionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    WalletId = table.Column<int>(type: "integer", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletTransaction", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CreditCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BankName = table.Column<string>(type: "text", nullable: true),
                    BankAccountHolder = table.Column<string>(type: "text", nullable: true),
                    BankCode = table.Column<string>(type: "text", nullable: true),
                    CustomerId = table.Column<int>(type: "integer", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditCards_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FollwerArtists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerId = table.Column<int>(type: "integer", nullable: true),
                    ArtistId = table.Column<int>(type: "integer", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollwerArtists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FollwerArtists_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artists",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FollwerArtists_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DocumentMainDiamonds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DocumentLink = table.Column<string>(type: "text", nullable: true),
                    DocumentTitle = table.Column<string>(type: "text", nullable: true),
                    DiamondId = table.Column<int>(type: "integer", nullable: true),
                    MainDiamondId = table.Column<int>(type: "integer", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentMainDiamonds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentMainDiamonds_MainDiamonds_MainDiamondId",
                        column: x => x.MainDiamondId,
                        principalTable: "MainDiamonds",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ImageMainDiamonds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ImageLink = table.Column<string>(type: "text", nullable: true),
                    DiamondId = table.Column<int>(type: "integer", nullable: true),
                    MainDiamondId = table.Column<int>(type: "integer", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageMainDiamonds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImageMainDiamonds_MainDiamonds_MainDiamondId",
                        column: x => x.MainDiamondId,
                        principalTable: "MainDiamonds",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DocumentMainShaphies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DocumentLink = table.Column<string>(type: "text", nullable: true),
                    DocumentTitle = table.Column<string>(type: "text", nullable: true),
                    ShaphieId = table.Column<int>(type: "integer", nullable: true),
                    MainShaphieId = table.Column<int>(type: "integer", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentMainShaphies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentMainShaphies_MainShaphies_MainShaphieId",
                        column: x => x.MainShaphieId,
                        principalTable: "MainShaphies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ImageMainShaphies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ImageLink = table.Column<string>(type: "text", nullable: true),
                    ShaphieId = table.Column<int>(type: "integer", nullable: true),
                    MainShaphieId = table.Column<int>(type: "integer", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageMainShaphies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImageMainShaphies_MainShaphies_MainShaphieId",
                        column: x => x.MainShaphieId,
                        principalTable: "MainShaphies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DocumentSecondaryDiamonds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DocumentLink = table.Column<string>(type: "text", nullable: true),
                    DocumentTitle = table.Column<string>(type: "text", nullable: true),
                    DiamondId = table.Column<int>(type: "integer", nullable: true),
                    SecondaryDiamondId = table.Column<int>(type: "integer", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentSecondaryDiamonds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentSecondaryDiamonds_SecondaryDiamonds_SecondaryDiamon~",
                        column: x => x.SecondaryDiamondId,
                        principalTable: "SecondaryDiamonds",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ImageSecondaryDiamonds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ImageLink = table.Column<string>(type: "text", nullable: true),
                    DiamondId = table.Column<int>(type: "integer", nullable: true),
                    SecondaryDiamondId = table.Column<int>(type: "integer", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageSecondaryDiamonds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImageSecondaryDiamonds_SecondaryDiamonds_SecondaryDiamondId",
                        column: x => x.SecondaryDiamondId,
                        principalTable: "SecondaryDiamonds",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DocumentSecondaryShaphies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DocumentLink = table.Column<string>(type: "text", nullable: true),
                    DocumentTitle = table.Column<string>(type: "text", nullable: true),
                    ShaphieId = table.Column<int>(type: "integer", nullable: true),
                    SecondaryShaphieId = table.Column<int>(type: "integer", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentSecondaryShaphies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentSecondaryShaphies_SecondaryShaphies_SecondaryShaphi~",
                        column: x => x.SecondaryShaphieId,
                        principalTable: "SecondaryShaphies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ImageSecondaryShaphies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ImageLink = table.Column<string>(type: "text", nullable: true),
                    ShaphieId = table.Column<int>(type: "integer", nullable: true),
                    SecondaryShaphieId = table.Column<int>(type: "integer", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageSecondaryShaphies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImageSecondaryShaphies_SecondaryShaphies_SecondaryShaphieId",
                        column: x => x.SecondaryShaphieId,
                        principalTable: "SecondaryShaphies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Lots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartPrice = table.Column<float>(type: "real", nullable: true),
                    CurrentPrice = table.Column<float>(type: "real", nullable: true),
                    FinalPriceSold = table.Column<float>(type: "real", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    BidIncrement = table.Column<float>(type: "real", nullable: true),
                    Deposit = table.Column<float>(type: "real", nullable: true),
                    BuyNowPrice = table.Column<float>(type: "real", nullable: true),
                    FloorFeePercent = table.Column<int>(type: "integer", nullable: true),
                    StartTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EndTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ActualEndTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsExtend = table.Column<bool>(type: "boolean", nullable: true),
                    HaveFinancialProof = table.Column<bool>(type: "boolean", nullable: true),
                    SellerId = table.Column<int>(type: "integer", nullable: true),
                    StaffId = table.Column<int>(type: "integer", nullable: true),
                    JewelryId = table.Column<int>(type: "integer", nullable: true),
                    AuctionId = table.Column<int>(type: "integer", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lots_Auctions_AuctionId",
                        column: x => x.AuctionId,
                        principalTable: "Auctions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Lots_Customers_SellerId",
                        column: x => x.SellerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Lots_Jewelries_JewelryId",
                        column: x => x.JewelryId,
                        principalTable: "Jewelries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Lots_Staffs_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staffs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BidPrices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CurrentPrice = table.Column<float>(type: "real", nullable: true),
                    BidTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CustomerId = table.Column<int>(type: "integer", nullable: true),
                    LotId = table.Column<int>(type: "integer", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BidPrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BidPrices_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BidPrices_Lots_LotId",
                        column: x => x.LotId,
                        principalTable: "Lots",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CustomerLots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BidType = table.Column<string>(type: "text", nullable: true),
                    CurrentPrice = table.Column<float>(type: "real", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    IsDeposit = table.Column<bool>(type: "boolean", nullable: true),
                    CustomerId = table.Column<int>(type: "integer", nullable: true),
                    LotId = table.Column<int>(type: "integer", nullable: true),
                    BidLimit = table.Column<float>(type: "real", nullable: true),
                    AutoBidPrice = table.Column<float>(type: "real", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerLots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerLots_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CustomerLots_Lots_LotId",
                        column: x => x.LotId,
                        principalTable: "Lots",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Watchings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerId = table.Column<int>(type: "integer", nullable: true),
                    LotId = table.Column<int>(type: "integer", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Watchings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Watchings_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Watchings_Lots_LotId",
                        column: x => x.LotId,
                        principalTable: "Lots",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AutoBids",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MinPrice = table.Column<float>(type: "real", nullable: true),
                    MaxPrice = table.Column<float>(type: "real", nullable: true),
                    NumberOfPriceStep = table.Column<float>(type: "real", nullable: true),
                    TimeIncrement = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: true),
                    CustomerLotId = table.Column<int>(type: "integer", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoBids", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AutoBids_CustomerLots_CustomerLotId",
                        column: x => x.CustomerLotId,
                        principalTable: "CustomerLots",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Status = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<float>(type: "real", nullable: true),
                    Free = table.Column<float>(type: "real", nullable: true),
                    FeeShip = table.Column<float>(type: "real", nullable: true),
                    TotalPrice = table.Column<float>(type: "real", nullable: true),
                    TransferInvoice = table.Column<string>(type: "text", nullable: true),
                    CustomerId = table.Column<int>(type: "integer", nullable: true),
                    CustomerLotId = table.Column<int>(type: "integer", nullable: true),
                    PaymentMethodId = table.Column<int>(type: "integer", nullable: true),
                    AddressToShipId = table.Column<int>(type: "integer", nullable: true),
                    StaffId = table.Column<int>(type: "integer", nullable: true),
                    ShipperId = table.Column<int>(type: "integer", nullable: true),
                    InvoiceOfWalletTransactionId = table.Column<int>(type: "integer", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_AddressToShips_AddressToShipId",
                        column: x => x.AddressToShipId,
                        principalTable: "AddressToShips",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Invoices_CustomerLots_CustomerLotId",
                        column: x => x.CustomerLotId,
                        principalTable: "CustomerLots",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Invoices_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Invoices_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Invoices_Staffs_ShipperId",
                        column: x => x.ShipperId,
                        principalTable: "Staffs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Invoices_Staffs_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staffs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Invoices_WalletTransaction_InvoiceOfWalletTransactionId",
                        column: x => x.InvoiceOfWalletTransactionId,
                        principalTable: "WalletTransaction",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StatusInvoices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Status = table.Column<string>(type: "text", nullable: true),
                    ImageLink = table.Column<string>(type: "text", nullable: true),
                    CurrentDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    InvoiceId = table.Column<int>(type: "integer", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatusInvoices_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Manager");

            migrationBuilder.CreateIndex(
                name: "IX_AutoBids_CustomerLotId",
                table: "AutoBids",
                column: "CustomerLotId");

            migrationBuilder.CreateIndex(
                name: "IX_BidPrices_CustomerId",
                table: "BidPrices",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_BidPrices_LotId",
                table: "BidPrices",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCards_CustomerId",
                table: "CreditCards",
                column: "CustomerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerLots_CustomerId",
                table: "CustomerLots",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerLots_LotId",
                table: "CustomerLots",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_AccountId",
                table: "Customers",
                column: "AccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentMainDiamonds_MainDiamondId",
                table: "DocumentMainDiamonds",
                column: "MainDiamondId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentMainShaphies_MainShaphieId",
                table: "DocumentMainShaphies",
                column: "MainShaphieId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSecondaryDiamonds_SecondaryDiamondId",
                table: "DocumentSecondaryDiamonds",
                column: "SecondaryDiamondId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSecondaryShaphies_SecondaryShaphieId",
                table: "DocumentSecondaryShaphies",
                column: "SecondaryShaphieId");

            migrationBuilder.CreateIndex(
                name: "IX_FollwerArtists_ArtistId",
                table: "FollwerArtists",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_FollwerArtists_CustomerId",
                table: "FollwerArtists",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoryValuations_ValuationId",
                table: "HistoryValuations",
                column: "ValuationId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageMainDiamonds_MainDiamondId",
                table: "ImageMainDiamonds",
                column: "MainDiamondId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageMainShaphies_MainShaphieId",
                table: "ImageMainShaphies",
                column: "MainShaphieId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageSecondaryDiamonds_SecondaryDiamondId",
                table: "ImageSecondaryDiamonds",
                column: "SecondaryDiamondId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageSecondaryShaphies_SecondaryShaphieId",
                table: "ImageSecondaryShaphies",
                column: "SecondaryShaphieId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_AddressToShipId",
                table: "Invoices",
                column: "AddressToShipId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_CustomerId",
                table: "Invoices",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_CustomerLotId",
                table: "Invoices",
                column: "CustomerLotId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceOfWalletTransactionId",
                table: "Invoices",
                column: "InvoiceOfWalletTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_PaymentMethodId",
                table: "Invoices",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_ShipperId",
                table: "Invoices",
                column: "ShipperId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_StaffId",
                table: "Invoices",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_Lots_AuctionId",
                table: "Lots",
                column: "AuctionId");

            migrationBuilder.CreateIndex(
                name: "IX_Lots_JewelryId",
                table: "Lots",
                column: "JewelryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lots_SellerId",
                table: "Lots",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_Lots_StaffId",
                table: "Lots",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_MainDiamonds_JewelryId",
                table: "MainDiamonds",
                column: "JewelryId");

            migrationBuilder.CreateIndex(
                name: "IX_MainShaphies_JewelryId",
                table: "MainShaphies",
                column: "JewelryId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_AccountId",
                table: "Notifications",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestWithdraws_WalletId",
                table: "RequestWithdraws",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_SecondaryDiamonds_JewelryId",
                table: "SecondaryDiamonds",
                column: "JewelryId");

            migrationBuilder.CreateIndex(
                name: "IX_SecondaryShaphies_JewelryId",
                table: "SecondaryShaphies",
                column: "JewelryId");

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_AccountId",
                table: "Staffs",
                column: "AccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StatusInvoices_InvoiceId",
                table: "StatusInvoices",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Watchings_CustomerId",
                table: "Watchings",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Watchings_LotId",
                table: "Watchings",
                column: "LotId");

            migrationBuilder.AddForeignKey(
                name: "FK_AddressToShips_Customers_CustomerId",
                table: "AddressToShips",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AddressToShips_Wards_WardId",
                table: "AddressToShips",
                column: "WardId",
                principalTable: "Wards",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BidLimits_Customers_CustomerId",
                table: "BidLimits",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Districts_Provinces_ProvinceId",
                table: "Districts",
                column: "ProvinceId",
                principalTable: "Provinces",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageJewelrys_Jewelries_JewelryId",
                table: "ImageJewelrys",
                column: "JewelryId",
                principalTable: "Jewelries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Valuations_Customers_SellerId",
                table: "Valuations",
                column: "SellerId",
                principalTable: "Customers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Valuations_Staffs_StaffId",
                table: "Valuations",
                column: "StaffId",
                principalTable: "Staffs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_Customers_CustomerId",
                table: "Wallets",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Wards_Districts_DistrictId",
                table: "Wards",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AddressToShips_Customers_CustomerId",
                table: "AddressToShips");

            migrationBuilder.DropForeignKey(
                name: "FK_AddressToShips_Wards_WardId",
                table: "AddressToShips");

            migrationBuilder.DropForeignKey(
                name: "FK_BidLimits_Customers_CustomerId",
                table: "BidLimits");

            migrationBuilder.DropForeignKey(
                name: "FK_Districts_Provinces_ProvinceId",
                table: "Districts");

            migrationBuilder.DropForeignKey(
                name: "FK_ImageJewelrys_Jewelries_JewelryId",
                table: "ImageJewelrys");

            migrationBuilder.DropForeignKey(
                name: "FK_Valuations_Customers_SellerId",
                table: "Valuations");

            migrationBuilder.DropForeignKey(
                name: "FK_Valuations_Staffs_StaffId",
                table: "Valuations");

            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_Customers_CustomerId",
                table: "Wallets");

            migrationBuilder.DropForeignKey(
                name: "FK_Wards_Districts_DistrictId",
                table: "Wards");

            migrationBuilder.DropTable(
                name: "AutoBids");

            migrationBuilder.DropTable(
                name: "BidIncrements");

            migrationBuilder.DropTable(
                name: "BidPrices");

            migrationBuilder.DropTable(
                name: "CreditCards");

            migrationBuilder.DropTable(
                name: "DocumentMainDiamonds");

            migrationBuilder.DropTable(
                name: "DocumentMainShaphies");

            migrationBuilder.DropTable(
                name: "DocumentSecondaryDiamonds");

            migrationBuilder.DropTable(
                name: "DocumentSecondaryShaphies");

            migrationBuilder.DropTable(
                name: "FeeShips");

            migrationBuilder.DropTable(
                name: "FloorFeePersents");

            migrationBuilder.DropTable(
                name: "FollwerArtists");

            migrationBuilder.DropTable(
                name: "HistoryValuations");

            migrationBuilder.DropTable(
                name: "ImageMainDiamonds");

            migrationBuilder.DropTable(
                name: "ImageMainShaphies");

            migrationBuilder.DropTable(
                name: "ImageSecondaryDiamonds");

            migrationBuilder.DropTable(
                name: "ImageSecondaryShaphies");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "RequestWithdraws");

            migrationBuilder.DropTable(
                name: "StatusInvoices");

            migrationBuilder.DropTable(
                name: "Watchings");

            migrationBuilder.DropTable(
                name: "MainDiamonds");

            migrationBuilder.DropTable(
                name: "MainShaphies");

            migrationBuilder.DropTable(
                name: "SecondaryDiamonds");

            migrationBuilder.DropTable(
                name: "SecondaryShaphies");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "CustomerLots");

            migrationBuilder.DropTable(
                name: "PaymentMethods");

            migrationBuilder.DropTable(
                name: "WalletTransaction");

            migrationBuilder.DropTable(
                name: "Lots");

            migrationBuilder.DropTable(
                name: "Auctions");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Staffs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Wards",
                table: "Wards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Provinces",
                table: "Provinces");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ImageJewelrys",
                table: "ImageJewelrys");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Districts",
                table: "Districts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AddressToShips",
                table: "AddressToShips");

            migrationBuilder.DropColumn(
                name: "CancelReason",
                table: "Valuations");

            migrationBuilder.DropColumn(
                name: "EstimatePriceMax",
                table: "Valuations");

            migrationBuilder.DropColumn(
                name: "DocumentLink",
                table: "ValuationDocuments");

            migrationBuilder.DropColumn(
                name: "SignatureCode",
                table: "ValuationDocuments");

            migrationBuilder.DropColumn(
                name: "BidForm",
                table: "Jewelries");

            migrationBuilder.DropColumn(
                name: "EstimatePriceMax",
                table: "Jewelries");

            migrationBuilder.DropColumn(
                name: "ForGender",
                table: "Jewelries");

            migrationBuilder.DropColumn(
                name: "Time_Bidding",
                table: "Jewelries");

            migrationBuilder.DropColumn(
                name: "DefaultImage",
                table: "ImageValuations");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "ImageValuations");

            migrationBuilder.DropColumn(
                name: "ImageLink",
                table: "ImageJewelrys");

            migrationBuilder.DropColumn(
                name: "ThumbnailImage",
                table: "ImageJewelrys");

            migrationBuilder.RenameTable(
                name: "Wards",
                newName: "Ward");

            migrationBuilder.RenameTable(
                name: "Provinces",
                newName: "Province");

            migrationBuilder.RenameTable(
                name: "ImageJewelrys",
                newName: "ImageJewelry");

            migrationBuilder.RenameTable(
                name: "Districts",
                newName: "District");

            migrationBuilder.RenameTable(
                name: "AddressToShips",
                newName: "AddressToShip");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "Wallets",
                newName: "AccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Wallets_CustomerId",
                table: "Wallets",
                newName: "IX_Wallets_AccountId");

            migrationBuilder.RenameColumn(
                name: "EstimatePriceMin",
                table: "Valuations",
                newName: "DesiredPrice");

            migrationBuilder.RenameColumn(
                name: "ValuationDocumentType",
                table: "ValuationDocuments",
                newName: "FileDocument");

            migrationBuilder.RenameColumn(
                name: "VideoLink",
                table: "Jewelries",
                newName: "FileVideo");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Jewelries",
                newName: "Condition");

            migrationBuilder.RenameColumn(
                name: "StartingPrice",
                table: "Jewelries",
                newName: "ReserverPrice");

            migrationBuilder.RenameColumn(
                name: "EstimatePriceMin",
                table: "Jewelries",
                newName: "FinalPrice");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "BidLimits",
                newName: "AccountId");

            migrationBuilder.RenameIndex(
                name: "IX_BidLimits_CustomerId",
                table: "BidLimits",
                newName: "IX_BidLimits_AccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Wards_DistrictId",
                table: "Ward",
                newName: "IX_Ward_DistrictId");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "ImageJewelry",
                newName: "File");

            migrationBuilder.RenameIndex(
                name: "IX_ImageJewelrys_JewelryId",
                table: "ImageJewelry",
                newName: "IX_ImageJewelry_JewelryId");

            migrationBuilder.RenameIndex(
                name: "IX_Districts_ProvinceId",
                table: "District",
                newName: "IX_District_ProvinceId");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "AddressToShip",
                newName: "AccountId");

            migrationBuilder.RenameIndex(
                name: "IX_AddressToShips_WardId",
                table: "AddressToShip",
                newName: "IX_AddressToShip_WardId");

            migrationBuilder.RenameIndex(
                name: "IX_AddressToShips_CustomerId",
                table: "AddressToShip",
                newName: "IX_AddressToShip_AccountId");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryDate",
                table: "Valuations",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Valuations",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ValuationDocumentTypeId",
                table: "ValuationDocuments",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GenderId",
                table: "Jewelries",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Accounts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CitizenIdentificationCard",
                table: "Accounts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Accounts",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Accounts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Accounts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IDExpirationDate",
                table: "Accounts",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IDIssuanceDate",
                table: "Accounts",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Accounts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicture",
                table: "Accounts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VNPayAccount",
                table: "Accounts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VNPayAccountName",
                table: "Accounts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VNPayBankCode",
                table: "Accounts",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ward",
                table: "Ward",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Province",
                table: "Province",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImageJewelry",
                table: "ImageJewelry",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_District",
                table: "District",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AddressToShip",
                table: "AddressToShip",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Genders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProofTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProofTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ValuationDocumentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValuationDocumentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Proofs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JewelryId = table.Column<int>(type: "integer", nullable: true),
                    ProofTypeId = table.Column<int>(type: "integer", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proofs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Proofs_Jewelries_JewelryId",
                        column: x => x.JewelryId,
                        principalTable: "Jewelries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Proofs_ProofTypes_ProofTypeId",
                        column: x => x.ProofTypeId,
                        principalTable: "ProofTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TransactionTypeId = table.Column<int>(type: "integer", nullable: true),
                    WalletId = table.Column<int>(type: "integer", nullable: true),
                    Amount = table.Column<float>(type: "real", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DeleteBy = table.Column<int>(type: "integer", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    ModificationBy = table.Column<int>(type: "integer", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    TransactionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_TransactionTypes_TransactionTypeId",
                        column: x => x.TransactionTypeId,
                        principalTable: "TransactionTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transactions_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallets",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Admin");

            migrationBuilder.CreateIndex(
                name: "IX_ValuationDocuments_ValuationDocumentTypeId",
                table: "ValuationDocuments",
                column: "ValuationDocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Jewelries_GenderId",
                table: "Jewelries",
                column: "GenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Proofs_JewelryId",
                table: "Proofs",
                column: "JewelryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Proofs_ProofTypeId",
                table: "Proofs",
                column: "ProofTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransactionTypeId",
                table: "Transactions",
                column: "TransactionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_WalletId",
                table: "Transactions",
                column: "WalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_AddressToShip_Accounts_AccountId",
                table: "AddressToShip",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AddressToShip_Ward_WardId",
                table: "AddressToShip",
                column: "WardId",
                principalTable: "Ward",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BidLimits_Accounts_AccountId",
                table: "BidLimits",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_District_Province_ProvinceId",
                table: "District",
                column: "ProvinceId",
                principalTable: "Province",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageJewelry_Jewelries_JewelryId",
                table: "ImageJewelry",
                column: "JewelryId",
                principalTable: "Jewelries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Jewelries_Genders_GenderId",
                table: "Jewelries",
                column: "GenderId",
                principalTable: "Genders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ValuationDocuments_ValuationDocumentTypes_ValuationDocument~",
                table: "ValuationDocuments",
                column: "ValuationDocumentTypeId",
                principalTable: "ValuationDocumentTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Valuations_Accounts_SellerId",
                table: "Valuations",
                column: "SellerId",
                principalTable: "Accounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Valuations_Accounts_StaffId",
                table: "Valuations",
                column: "StaffId",
                principalTable: "Accounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_Accounts_AccountId",
                table: "Wallets",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ward_District_DistrictId",
                table: "Ward",
                column: "DistrictId",
                principalTable: "District",
                principalColumn: "Id");
        }
    }
}
