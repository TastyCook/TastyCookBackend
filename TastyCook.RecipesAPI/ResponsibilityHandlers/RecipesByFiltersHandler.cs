namespace TastyCook.RecipesAPI.ResponsibilityHandlers
{
    public class RecipesByFiltersHandler : AbstractHandler
    {
        public override HandlersRequest Handle(HandlersRequest request)
        {
            if (request.Localization != Localization.None)
            {
                request.Recipes = request.Recipes.Where(r => r.Localization == request.Localization);
            }

            if (!string.IsNullOrWhiteSpace(request.LikedUserId))
            {
                request.Recipes = request.Recipes.Where(r => r.RecipeUsers.Any(ru => ru.UserId == request.LikedUserId && ru.IsUserLiked));
            }

            if (!string.IsNullOrEmpty(request.Email))
            {
                request.Recipes = request.Recipes.Where(r => r.User.Email == request.Email);
            }

            if (!string.IsNullOrEmpty(request.SearchValue) && request.Filters.Any())
            {
                request.Recipes = request.Recipes.Where(r => (r.Name.Contains(request.SearchValue) || r.Description.Contains(request.SearchValue))
                                                             && r.Categories.Any(c => request.Filters.Any(f => f == c.Name)));
            }
            else if (!string.IsNullOrEmpty(request.SearchValue) && !request.Filters.Any())
            {
                request.Recipes = request.Recipes.Where(r => r.Name.Contains(request.SearchValue) || r.Description.Contains(request.SearchValue));
            }
            else if (request.Filters.Any())
            {
                request.Recipes = request.Recipes.Where(r => r.Categories.Any(c => request.Filters.Any(f => f == c.Name)));
            }

            return base.Handle(request);
        }
    }
}
