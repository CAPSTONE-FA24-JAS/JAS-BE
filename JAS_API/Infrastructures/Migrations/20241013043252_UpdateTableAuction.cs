using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableAuction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Auctions");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "Auctions",
                newName: "ImageLink");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageLink",
                table: "Auctions",
                newName: "Notes");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Auctions",
                type: "text",
                nullable: true);
        }
    }
}
