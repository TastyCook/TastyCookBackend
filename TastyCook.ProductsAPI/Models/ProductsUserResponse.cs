namespace TastyCook.ProductsAPI.Models;

public class ProductsUserResponse
{
    public IEnumerable<AllUserProducts> Products { get; set; }
    public int TotalPagesWithCurrentLimit { get; set; }
}