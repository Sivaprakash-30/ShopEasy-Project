using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopEasy.Migrations
{
    /// <inheritdoc />
    public partial class MigOrder3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Customer_Reg_CustomerRegRegId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_ProductTypes_pid",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CustomerRegRegId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_pid",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CustomerRegRegId",
                table: "Orders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerRegRegId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerRegRegId",
                table: "Orders",
                column: "CustomerRegRegId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_pid",
                table: "Orders",
                column: "pid");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Customer_Reg_CustomerRegRegId",
                table: "Orders",
                column: "CustomerRegRegId",
                principalTable: "Customer_Reg",
                principalColumn: "RegID");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_ProductTypes_pid",
                table: "Orders",
                column: "pid",
                principalTable: "ProductTypes",
                principalColumn: "pid",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
