using TastyCook.RecipesAPI.Models;

namespace TastyCook.RecipesAPI.ResponsibilityHandlers;
public interface IHandler
{
    IHandler SetNext(IHandler handler);

    HandlersRequest Handle(HandlersRequest request);
}