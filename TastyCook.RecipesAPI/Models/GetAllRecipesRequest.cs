namespace TastyCook.RecipesAPI.Models;

public class GetAllRecipesRequest
{
    public int? Offset { get; set; }
    public int? Limit { get; set; }
    public string? SearchValue { get; set; }
    public string[] Filters { get; set; } = Array.Empty<string>();
    public Localization Localization { get; set; }
}