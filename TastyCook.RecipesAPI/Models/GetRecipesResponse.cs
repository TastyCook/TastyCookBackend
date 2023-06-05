using TastyCook.RecipesAPI.Entities;

namespace TastyCook.RecipesAPI.Models;

public class GetRecipesResponse
{
    public IEnumerable<RecipeModel> Recipes { get; set; }
    public int TotalPagesWithCurrentLimit { get; set; }
}