using MassTransit;
using TastyCook.RecepiesAPI.Models;
using static Contracts.Contracts;

namespace TastyCook.RecepiesAPI.Consumers;

public class UserDeletedConsumer : IConsumer<UserItemDeleted>
{
    private readonly UserService _userService;

    public UserDeletedConsumer(UserService userService)
    {
        _userService = userService;
    }

    public async Task Consume(ConsumeContext<UserItemDeleted> context)
    {
        var message = context.Message;
        await Console.Out.WriteLineAsync($"Message from Producer : {message.id}");

        _userService.DeleteById(message.id);

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