using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class updatetabletransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WalletId",
                table: "WalletTransaction");

            migrationBuilder.AddColumn<int>(
                name: "transactionPerson",
                table: "WalletTransaction",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "transactionType",
                table: "WalletTransaction",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "transactionPerson",
                table: "WalletTransaction");

            migrationBuilder.DropColumn(
                name: "transactionType",
                table: "WalletTransaction");

            migrationBuilder.AddColumn<int>(
                name: "WalletId",
                table: "WalletTransaction",
                type: "integer",
                nullable: true);
        }
    }
}
