using TastyCook.RecipesAPI.Entities;

namespace TastyCook.RecipesAPI.ResponsibilityHandlers;

public class HandlersRequest
{
    public int? Offset { get; set; }
    public int? Limit { get; set; }
    public string? SearchValue { get; set; }
    public string[] Filters { get; set; } = Array.Empty<string>();
    public Localization Localization { get; set; }
    public IQueryable<Recipe> Recipes { get; set; }
    public string LikedUserId { get; set; }
    public string Email { get; set; }
}