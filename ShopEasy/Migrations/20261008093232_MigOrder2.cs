using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopEasy.Migrations
{
    /// <inheritdoc />
    public partial class MigOrder2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Oid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    pid = table.Column<int>(type: "int", nullable: false),
                    orderdate = table.Column<int>(type: "int", nullable: false),
                    totalprice = table.Column<int>(type: "int", nullable: false),
                    CustomerRegRegId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_Orders_Customer_Reg_CustomerRegRegId",
                        column: x => x.CustomerRegRegId,
                        principalTable: "Customer_Reg",
                        principalColumn: "RegID");
                    table.ForeignKey(
                        name: "FK_Orders_ProductTypes_pid",
                        column: x => x.pid,
                        principalTable: "ProductTypes",
                        principalColumn: "pid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerRegRegId",
                table: "Orders",
                column: "CustomerRegRegId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_pid",
                table: "Orders",
                column: "pid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
