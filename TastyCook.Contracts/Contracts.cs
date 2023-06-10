namespace TastyCook.Contracts
{
    public class Contracts
    {
        public record UserItemCreated(string Id, string Email, string Username);
        public record UserItemUpdated(string Id, string Email, string Username);
        public record UserItemDeleted(string Id);


        public record RecipeItemCreated(string Id, string Title, string Description);
        public record RecipeItemUpdated(string Id, string Email, string Username);
        public record RecipeItemDeleted(string Id);
    }
}