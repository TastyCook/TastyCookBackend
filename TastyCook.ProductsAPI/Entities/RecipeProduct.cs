using Microsoft.EntityFrameworkCore;

namespace TastyCook.ProductsAPI.Entities;

[PrimaryKey(nameof(ProductId), nameof(RecipeId))]
public class RecipeProduct
{
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; }
    public string Amount { get; set; }
}