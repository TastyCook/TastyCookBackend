namespace TastyCook.UsersAPI.Settings
{
    public class RabbitMQSettingsOptions
    {
        public const string RabbitMQSettings = "RabbitMQSettings";
        public string Host { get; set; }
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
