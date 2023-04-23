using MassTransit;
using TastyCook.RecepiesAPI;
using TastyCook.RecepiesAPI.Models;
using static Contracts.Contracts;

namespace TastyCook.RecepiesAPI.Consumers;

public class UserCreatedConsumer : IConsumer<UserItemCreated>
{
        private readonly UserService _userService;

        public UserCreatedConsumer(UserService userService)
        {
            _userService = userService;
        }

        public async Task Consume(ConsumeContext<UserItemCreated> context)
        {
            var message = context.Message;
            await Console.Out.WriteLineAsync($"Message from Producer : {message.email}");
        //var item = _userService.GetById(message.id);

        //if (item != null)
        //{
        //    return;
        //}

        //item = new User()
        //{
        //    Email = 
        //}

        //await repository.CreateAsync(item);
    }
}