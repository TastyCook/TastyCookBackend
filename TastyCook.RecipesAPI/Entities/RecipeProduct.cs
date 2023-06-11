namespace TastyCook.RecipesAPI.Entities;

public class RecipeProduct
{
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; }
    public string Amount { get; set; }
}