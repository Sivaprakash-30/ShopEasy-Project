using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopEasy.Migrations
{
    /// <inheritdoc />
    public partial class MigOrderaddAddessid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "addressId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "orderdate",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "addressId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "orderdate",
                table: "Orders");
        }
    }
}
