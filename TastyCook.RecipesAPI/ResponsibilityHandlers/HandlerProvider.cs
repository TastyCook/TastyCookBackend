namespace TastyCook.RecipesAPI.ResponsibilityHandlers;

public class HandlerProvider
{
    public static IHandler GetFiltersHandler()
    {
        var paginationHandler = new PaginationHandler();

        IHandler handler = new RecipesByFiltersHandler();
        handler = handler.SetNext(paginationHandler);

        return handler;
    }

}