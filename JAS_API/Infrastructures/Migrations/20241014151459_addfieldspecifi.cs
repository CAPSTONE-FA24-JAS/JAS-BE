using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class addfieldspecifi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "SpecificPrice",
                table: "Valuations",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "SpecificPrice",
                table: "Jewelries",
                type: "real",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpecificPrice",
                table: "Valuations");

            migrationBuilder.DropColumn(
                name: "SpecificPrice",
                table: "Jewelries");
        }
    }
}
