namespace TastyCook.ProductsAPI.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Calories { get; set; }
    public Localization Localization { get; set; }
    public IEnumerable<ProductUser> ProductUsers { get; set; }
    public IEnumerable<RecipeProduct> RecipeProducts { get; set; }
}