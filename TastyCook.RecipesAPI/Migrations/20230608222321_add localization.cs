using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TastyCook.RecipesAPI.Migrations
{
    public partial class addlocalization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Localization",
                table: "Recipes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Localization",
                table: "Recipes");
        }
    }
}
