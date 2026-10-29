using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopEasy.Migrations
{
    /// <inheritdoc />
    public partial class migoneToManyRelation : Migration
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

            migrationBuilder.CreateIndex(
                name: "IX_wishlists_Pid",
                table: "wishlists",
                column: "Pid");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_Pid",
                table: "Carts",
                column: "Pid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_wishlists_Pid",
                table: "wishlists");

            migrationBuilder.DropIndex(
                name: "IX_Carts_Pid",
                table: "Carts");

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
    }
}
