using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopEasy.Migrations
{
    /// <inheritdoc />
    public partial class Migaddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_wishlists_Pid",
                table: "wishlists");

            migrationBuilder.DropIndex(
                name: "IX_Carts_Pid",
                table: "Carts");

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    AddressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.AddressId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_wishlists_Pid",
                table: "wishlists",
                column: "Pid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Carts_Pid",
                table: "Carts",
                column: "Pid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_wishlists_Pid",
                table: "wishlists");

            migrationBuilder.DropIndex(
                name: "IX_Carts_Pid",
                table: "Carts");

            migrationBuilder.CreateIndex(
                name: "IX_wishlists_Pid",
                table: "wishlists",
                column: "Pid");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_Pid",
                table: "Carts",
                column: "Pid");
        }
    }
}
