using TastyCook.RecipesAPI.Entities;

namespace TastyCook.RecipesAPI.Models;

public class CommentModel
{
    public int Id { get; set; }
    public string CommentValue { get; set; }
    public int RecipeId { get; set; }
    public string? Username { get; set; }
}