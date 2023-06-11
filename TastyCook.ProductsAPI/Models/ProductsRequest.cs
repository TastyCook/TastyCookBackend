namespace TastyCook.ProductsAPI.Models;

public class ProductsRequest
{
    public int? Offset { get; set; }
    public int? Limit { get; set; }
    public string? SearchValue { get; set; }
    public Localization Localization { get; set; }
}