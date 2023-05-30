namespace TastyCook.Contracts
{
    public class Contracts
    {
        public record UserItemCreated(string Id, string Email, string Username, string Password);
        public record UserItemUpdated(string Id, string Email, string Username, string Password);
        public record UserItemDeleted(string Id);
    }
}