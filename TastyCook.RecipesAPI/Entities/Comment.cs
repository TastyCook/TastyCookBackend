namespace TastyCook.RecipesAPI.Entities;

public class Comment
{
    public int Id { get; set; }
    public string CommentValue { get; set; }
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; }
    public string UserId { get; set; }
    public User? User { get; set; }
}