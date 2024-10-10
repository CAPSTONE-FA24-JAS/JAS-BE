using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class addfieldintableValuation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppraiserId",
                table: "Valuations",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Valuations_AppraiserId",
                table: "Valuations",
                column: "AppraiserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Valuations_Staffs_AppraiserId",
                table: "Valuations",
                column: "AppraiserId",
                principalTable: "Staffs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Valuations_Staffs_AppraiserId",
                table: "Valuations");

            migrationBuilder.DropIndex(
                name: "IX_Valuations_AppraiserId",
                table: "Valuations");

            migrationBuilder.DropColumn(
                name: "AppraiserId",
                table: "Valuations");
        }
    }
}
