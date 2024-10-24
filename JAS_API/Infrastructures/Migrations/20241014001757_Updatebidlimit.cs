using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class Updatebidlimit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpireDateOfBidLimit",
                table: "CustomerLots",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "PriceLimit",
                table: "CustomerLots",
                type: "real",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpireDateOfBidLimit",
                table: "CustomerLots");

            migrationBuilder.DropColumn(
                name: "PriceLimit",
                table: "CustomerLots");
        }
    }
}
