using TastyCook.RecipesAPI.Entities;

namespace TastyCook.RecipesAPI.Models;

public class GetRecipesResponse
{
    public IEnumerable<Recipe> Recipes { get; set; }
    public int TotalPagesWithCurrentLimit { get; set; }
}