using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class Changefieldintablecustomerlot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BidType",
                table: "CustomerLots");

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoBid",
                table: "CustomerLots",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAutoBid",
                table: "CustomerLots");

            migrationBuilder.AddColumn<string>(
                name: "BidType",
                table: "CustomerLots",
                type: "text",
                nullable: true);
        }
    }
}
