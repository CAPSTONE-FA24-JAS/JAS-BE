using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class fixfluent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Lots_JewelryId",
                table: "Lots");

            migrationBuilder.CreateIndex(
                name: "IX_Lots_JewelryId",
                table: "Lots",
                column: "JewelryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Lots_JewelryId",
                table: "Lots");

            migrationBuilder.CreateIndex(
                name: "IX_Lots_JewelryId",
                table: "Lots",
                column: "JewelryId",
                unique: true);
        }
    }
}
