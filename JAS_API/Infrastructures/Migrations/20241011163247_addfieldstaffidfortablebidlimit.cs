using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class addfieldstaffidfortablebidlimit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Jewelries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StaffId",
                table: "BidLimits",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Auctions",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Jewelries");

            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "BidLimits");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Auctions");
        }
    }
}
