namespace TastyCook.ProductsAPI.Models;

public class ProductModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Calories { get; set; }
    public Localization Localization { get; set; }
}