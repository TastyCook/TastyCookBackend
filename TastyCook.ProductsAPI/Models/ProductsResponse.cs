using TastyCook.ProductsAPI.Entities;

namespace TastyCook.ProductsAPI.Models;

public class ProductsResponse
{
    public IEnumerable<AllProducts> Products { get; set; }
    public int TotalPagesWithCurrentLimit { get; set; }
}