namespace TastyCook.Contracts
{
    public class Contracts
    {
        public record UserItemCreated(string id, string email, string password);
        public record UserItemUpdated(string id, string email, string password);
        public record UserItemDeleted(string id);
    }
}