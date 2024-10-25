using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class deletewardinaddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AddressToShips_Wards_WardId",
                table: "AddressToShips");

            migrationBuilder.DropIndex(
                name: "IX_AddressToShips_WardId",
                table: "AddressToShips");

            migrationBuilder.DropColumn(
                name: "WardId",
                table: "AddressToShips");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WardId",
                table: "AddressToShips",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AddressToShips_WardId",
                table: "AddressToShips",
                column: "WardId");

            migrationBuilder.AddForeignKey(
                name: "FK_AddressToShips_Wards_WardId",
                table: "AddressToShips",
                column: "WardId",
                principalTable: "Wards",
                principalColumn: "Id");
        }
    }
}
