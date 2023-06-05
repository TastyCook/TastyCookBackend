using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TastyCook.RecipesAPI.Migrations
{
    public partial class adduserslikes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecipeUsers",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: false),
                    RecipeId = table.Column<int>(type: "int", nullable: false),
                    IsUserLiked = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeUsers", x => new { x.UserId, x.RecipeId });
                    table.ForeignKey(
                        name: "FK_RecipeUsers_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RecipeUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecipeUsers_RecipeId",
                table: "RecipeUsers",
                column: "RecipeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecipeUsers");
        }
    }
}
