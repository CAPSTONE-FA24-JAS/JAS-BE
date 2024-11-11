using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class updatetableAutobid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Chuyển đổi DateTime sang integer bằng cách tính Unix timestamp
            migrationBuilder.Sql("ALTER TABLE \"AutoBids\" ALTER COLUMN \"TimeIncrement\" TYPE integer USING EXTRACT(EPOCH FROM \"TimeIncrement\")::integer;");
        }




        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeIncrement",
                table: "AutoBids",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
