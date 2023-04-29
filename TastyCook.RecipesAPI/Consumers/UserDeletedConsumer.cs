using MassTransit;
using TastyCook.RecipesAPI.Models;
using static TastyCook.Contracts.Contracts;

namespace TastyCook.RecipesAPI.Consumers;

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
        await Console.Out.WriteLineAsync($"Message from Producer : {message.Id}");

        _userService.DeleteById(message.Id);

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