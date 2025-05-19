using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Laptops.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeOrderIdNullableInCartItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Example change (if this was the original intention):
            migrationBuilder.AlterColumn<int>(
                name: "order_id",
                table: "cart_items",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert the change if rolling back
            migrationBuilder.AlterColumn<int>(
                name: "order_id",
                table: "cart_items",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
