using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopEasy.Migrations
{
    /// <inheritdoc />
    public partial class MigFeedback1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Pid",
                table: "Feedbacks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pid",
                table: "Feedbacks");
        }
    }
}
