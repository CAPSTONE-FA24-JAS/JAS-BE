using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class addfieldlimitpricetoCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpireDate",
                table: "Customers",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "PriceLimit",
                table: "Customers",
                type: "real",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpireDate",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PriceLimit",
                table: "Customers");
        }
    }
}
