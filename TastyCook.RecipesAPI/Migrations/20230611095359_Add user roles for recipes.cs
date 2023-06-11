using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TastyCook.RecipesAPI.Migrations
{
    public partial class Adduserrolesforrecipes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            //migrationBuilder.AddColumn<string>(
            //    name: "ImageUrl",
            //    table: "Recipes",
            //    type: "nvarchar(max)",
            //    nullable: true);

            //migrationBuilder.AddColumn<int>(
            //    name: "Localization",
            //    table: "Recipes",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0);

            //migrationBuilder.AddColumn<int>(
            //    name: "Localization",
            //    table: "Categories",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0);

            //migrationBuilder.CreateTable(
            //    name: "Comments",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CommentValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        RecipeId = table.Column<int>(type: "int", nullable: false),
            //        UserId = table.Column<string>(type: "nvarchar(225)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Comments", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_Comments_Recipes_RecipeId",
            //            column: x => x.RecipeId,
            //            principalTable: "Recipes",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_Comments_Users_UserId",
            //            column: x => x.UserId,
            //            principalTable: "Users",
            //            principalColumn: "Id");
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_Comments_RecipeId",
            //    table: "Comments",
            //    column: "RecipeId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Comments_UserId",
            //    table: "Comments",
            //    column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "Comments");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            //migrationBuilder.DropColumn(
            //    name: "ImageUrl",
            //    table: "Recipes");

            //migrationBuilder.DropColumn(
            //    name: "Localization",
            //    table: "Recipes");

            //migrationBuilder.DropColumn(
            //    name: "Localization",
            //    table: "Categories");
        }
    }
}
