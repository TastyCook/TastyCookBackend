using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TastyCook.ProductsAPI.Migrations
{
    /// <inheritdoc />
    public partial class addproductslocalization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Localization",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Localization",
                table: "Products");
        }
    }
}
