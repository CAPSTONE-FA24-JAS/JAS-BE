using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class updaterelationshipofAddresswithInvoi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invoices_AddressToShipId",
                table: "Invoices");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_AddressToShipId",
                table: "Invoices",
                column: "AddressToShipId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invoices_AddressToShipId",
                table: "Invoices");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_AddressToShipId",
                table: "Invoices",
                column: "AddressToShipId",
                unique: true);
        }
    }
}
