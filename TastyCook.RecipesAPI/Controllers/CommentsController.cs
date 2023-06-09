using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TastyCook.RecipesAPI.Entities;
using TastyCook.RecipesAPI.Models;
using TastyCook.RecipesAPI.Services;

namespace TastyCook.RecipesAPI.Controllers;

[ApiController]
[Route("/api/[controller]")]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly ILogger<CommentsController> _logger;
    private readonly CommentsService _commentsService;
    private readonly UserService _userService;

    public CommentsController(CommentsService commentsService,
        ILogger<CommentsController> logger,
        UserService userService)
    {
        _commentsService = commentsService;
        _logger = logger;
        _userService = userService;
    }

    [HttpGet]
    [Route("{recipeId}")]
    [AllowAnonymous]
    public IActionResult GetAllCommentsByRecipe(int recipeId)
    {
        try
        {
            _logger.LogInformation($"{DateTime.Now} | Start getting all comments, recipeId: {recipeId}");
            var comments = MapCommentToModel(_commentsService.GetByRecipeId(recipeId));
            _logger.LogInformation($"{DateTime.Now} | End getting all comments, recipeId: {recipeId}");

            return Ok(comments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    [Route("")]
    public IActionResult AddComment([FromBody] CommentModel model)
    {
        try
        {
            _logger.LogInformation($"{DateTime.Now} | Start adding new comment, recipeId: {model.RecipeId}");
            var user = _userService.GetByEmail(User.Identity.Name);
            model.Username = user.UserName;
            _commentsService.Add(model);
            _logger.LogInformation($"{DateTime.Now} | End adding new comment, recipeId: {model.RecipeId}");

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPatch]
    [Route("{commentId}")]
    public IActionResult UpdateComment(int commentId, [FromBody] CommentModel model)
    {
        try
        {
            _logger.LogInformation($"{DateTime.Now} | Start updating new comment, recipeId: {model.RecipeId}");
            model.Id = commentId;
            var user = _userService.GetByEmail(User.Identity.Name);
            model.Username = user.UserName;
            _commentsService.Update(model);
            _logger.LogInformation($"{DateTime.Now} | End updating new comment, recipeId: {model.RecipeId}");

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete]
    [Route("{commentId}")]
    public IActionResult DeleteComment(int commentId)
    {
        try
        {
            _logger.LogInformation($"{DateTime.Now} | Start deleting comment, commentId: {commentId}");
            _commentsService.DeleteById(commentId);
            _logger.LogInformation($"{DateTime.Now} | End deleting comment, commentId: {commentId}");

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    public CommentModel MapCommentToModel(Comment comment)
    {
        var commentModel = new CommentModel()
        {
            Id = comment.Id,
            RecipeId = comment.RecipeId,
            CommentValue = comment.CommentValue,
            Username = comment.User?.UserName
        };

        return commentModel;
    }

    public IEnumerable<CommentModel> MapCommentToModel(IEnumerable<Comment> comments)
    {
        var commentModels = comments.Select(c => new CommentModel()
        {
            Id = c.Id,
            RecipeId = c.RecipeId,
            CommentValue = c.CommentValue,
            Username = c.User?.UserName
        }).ToList();

        return commentModels;
    }
}