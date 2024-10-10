using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class updatetablewalletandcreditcard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE \"Wallets\" ALTER COLUMN \"Balance\" TYPE numeric USING \"Balance\"::numeric;");
            migrationBuilder.AlterColumn<decimal>(
                name: "Balance",
                table: "Wallets",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Wallets",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE \"Wallets\" ALTER COLUMN \"Balance\" TYPE text USING \"Balance\"::text;");
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Wallets");

            migrationBuilder.AlterColumn<string>(
                name: "Balance",
                table: "Wallets",
                type: "text",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);
        }
    }
}
