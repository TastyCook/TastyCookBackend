namespace TastyCook.Contracts
{
    public class Contracts
    {
        public record UserItemCreated(string Id, string Email, string Username, string Role);
        public record UserItemUpdated(string Id, string Email, string Username, string Role);
        public record UserItemDeleted(string Id);


        public record ProductItemCreated(int Id, string Name, double Calories, Localization Localization);
        public record ProductItemUpdated(int Id, string Name, double Calories, Localization Localization);
        public record ProductItemDeleted(int Id);


        public record ProductUserItemCreated(string UserId, int ProductId, string Amount, string Type);
        public record ProductUserItemUpdated(string UserId, int ProductId, string Amount, string Type);
        public record ProductUserItemDeleted(string UserId, int ProductId);
    }
}