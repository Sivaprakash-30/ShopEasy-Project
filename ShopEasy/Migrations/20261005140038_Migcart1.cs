using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopEasy.Migrations
{
    /// <inheritdoc />
    public partial class Migcart1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    Cid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.Cid);
                    table.ForeignKey(
                        name: "FK_Carts_ProductTypes_Pid",
                        column: x => x.Pid,
                        principalTable: "ProductTypes",
                        principalColumn: "pid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Carts_Pid",
                table: "Carts",
                column: "Pid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Carts");
        }
    }
}
