using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class AddfieldTotableWalletTrans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WalletId",
                table: "WalletTransactions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Invoices",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_WalletId",
                table: "WalletTransactions",
                column: "WalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_Wallets_WalletId",
                table: "WalletTransactions",
                column: "WalletId",
                principalTable: "Wallets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_Wallets_WalletId",
                table: "WalletTransactions");

            migrationBuilder.DropIndex(
                name: "IX_WalletTransactions_WalletId",
                table: "WalletTransactions");

            migrationBuilder.DropColumn(
                name: "WalletId",
                table: "WalletTransactions");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Invoices");
        }
    }
}
