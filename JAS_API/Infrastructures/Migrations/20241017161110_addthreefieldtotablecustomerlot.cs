using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class addthreefieldtotablecustomerlot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsInvoiced",
                table: "CustomerLots",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRefunded",
                table: "CustomerLots",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsWinner",
                table: "CustomerLots",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInvoiced",
                table: "CustomerLots");

            migrationBuilder.DropColumn(
                name: "IsRefunded",
                table: "CustomerLots");

            migrationBuilder.DropColumn(
                name: "IsWinner",
                table: "CustomerLots");
        }
    }
}
