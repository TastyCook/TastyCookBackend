namespace TastyCook.RecipesAPI.ResponsibilityHandlers
{
    public class PaginationHandler : AbstractHandler
    {
        public override HandlersRequest Handle(HandlersRequest request)
        {
            if (request.Limit.HasValue && request.Offset.HasValue)
            {
                request.Recipes = request.Recipes.Skip(request.Offset.Value).Take(request.Limit.Value);
            }
            else if (request.Limit.HasValue && !request.Offset.HasValue)
            {
                request.Recipes = request.Recipes.Take(request.Limit.Value);
            }
            else if (!request.Limit.HasValue && request.Offset.HasValue)
            {
                request.Recipes = request.Recipes.Skip(request.Offset.Value);
            }

            return base.Handle(request);
        }
    }
}