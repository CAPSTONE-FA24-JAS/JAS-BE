using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class updatetablewatching : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Watchings_Lots_LotId",
                table: "Watchings");

            migrationBuilder.RenameColumn(
                name: "LotId",
                table: "Watchings",
                newName: "JewelryId");

            migrationBuilder.RenameIndex(
                name: "IX_Watchings_LotId",
                table: "Watchings",
                newName: "IX_Watchings_JewelryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Watchings_Jewelries_JewelryId",
                table: "Watchings",
                column: "JewelryId",
                principalTable: "Jewelries",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Watchings_Jewelries_JewelryId",
                table: "Watchings");

            migrationBuilder.RenameColumn(
                name: "JewelryId",
                table: "Watchings",
                newName: "LotId");

            migrationBuilder.RenameIndex(
                name: "IX_Watchings_JewelryId",
                table: "Watchings",
                newName: "IX_Watchings_LotId");

            migrationBuilder.AddForeignKey(
                name: "FK_Watchings_Lots_LotId",
                table: "Watchings",
                column: "LotId",
                principalTable: "Lots",
                principalColumn: "Id");
        }
    }
}
