using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableDiamondAndShie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Valuations",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Dimension",
                table: "SecondaryShaphies",
                type: "text",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "SecondaryShaphies",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "TotalCarat",
                table: "SecondaryShaphies",
                type: "real",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LengthWidthRatio",
                table: "SecondaryDiamonds",
                type: "text",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Dimension",
                table: "SecondaryDiamonds",
                type: "text",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "SecondaryDiamonds",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "TotalCarat",
                table: "SecondaryDiamonds",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "SecondaryDiamonds",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Dimension",
                table: "MainShaphies",
                type: "text",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "MainShaphies",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LengthWidthRatio",
                table: "MainDiamonds",
                type: "text",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Dimension",
                table: "MainDiamonds",
                type: "text",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "MainDiamonds",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "MainDiamonds",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ValuationId",
                table: "Jewelries",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Valuations_CustomerId",
                table: "Valuations",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Jewelries_ValuationId",
                table: "Jewelries",
                column: "ValuationId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Jewelries_Valuations_ValuationId",
                table: "Jewelries",
                column: "ValuationId",
                principalTable: "Valuations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Valuations_Customers_CustomerId",
                table: "Valuations",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jewelries_Valuations_ValuationId",
                table: "Jewelries");

            migrationBuilder.DropForeignKey(
                name: "FK_Valuations_Customers_CustomerId",
                table: "Valuations");

            migrationBuilder.DropIndex(
                name: "IX_Valuations_CustomerId",
                table: "Valuations");

            migrationBuilder.DropIndex(
                name: "IX_Jewelries_ValuationId",
                table: "Jewelries");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Valuations");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "SecondaryShaphies");

            migrationBuilder.DropColumn(
                name: "TotalCarat",
                table: "SecondaryShaphies");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "SecondaryDiamonds");

            migrationBuilder.DropColumn(
                name: "TotalCarat",
                table: "SecondaryDiamonds");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "SecondaryDiamonds");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "MainShaphies");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "MainDiamonds");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "MainDiamonds");

            migrationBuilder.DropColumn(
                name: "ValuationId",
                table: "Jewelries");

            migrationBuilder.AlterColumn<float>(
                name: "Dimension",
                table: "SecondaryShaphies",
                type: "real",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "LengthWidthRatio",
                table: "SecondaryDiamonds",
                type: "real",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "Dimension",
                table: "SecondaryDiamonds",
                type: "real",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "Dimension",
                table: "MainShaphies",
                type: "real",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "LengthWidthRatio",
                table: "MainDiamonds",
                type: "real",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "Dimension",
                table: "MainDiamonds",
                type: "real",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
