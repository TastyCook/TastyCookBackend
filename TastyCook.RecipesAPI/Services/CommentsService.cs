using Microsoft.EntityFrameworkCore;
using TastyCook.RecipesAPI.Entities;
using TastyCook.RecipesAPI.Models;

namespace TastyCook.RecipesAPI.Services;

public class CommentsService
{
    private readonly RecipesContext _db;

    public CommentsService(RecipesContext db)
    {
        _db = db;
    }

    public IEnumerable<Comment> GetByRecipeId(int recipeId)
    {
        return _db.Comments.Include(c => c.User).Where(c => c.RecipeId == recipeId).ToList();
    }

    public void Add(CommentModel commentModel)
    {
        var user = _db.Users.First(u => u.UserName == commentModel.Username);
        var comment = new Comment()
        {
            CommentValue = commentModel.CommentValue,
            RecipeId = commentModel.RecipeId,
            UserId = user.Id
        };

        _db.Comments.Add(comment);
        _db.SaveChanges();
    }

    public void Update(CommentModel commentModel)
    {
        var commentDb = _db.Comments.Find(commentModel.Id);
        commentDb.CommentValue = commentModel.CommentValue;
        _db.SaveChanges();
    }

    public void DeleteById(int id)
    {
        var commentToDelete = _db.Comments.FirstOrDefault(r => r.Id == id);
        if (commentToDelete != null)
        {
            _db.Comments.Remove(commentToDelete);
            _db.SaveChanges();
        }
    }
}