namespace TastyCook.RecipesAPI.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<Recipe> Recipes { get; set; }
    public Localization Localization { get; set; }
}