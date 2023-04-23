namespace Contracts
{
    public class Contracts
    {
        public record RecipeItemCreated(int id, string title, string description);
        public record UserItemCreated(string id, string email, string password);
    }
}