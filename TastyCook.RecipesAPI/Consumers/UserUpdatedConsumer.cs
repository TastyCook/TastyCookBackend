using MassTransit;
using TastyCook.RecipesAPI.Entities;
using TastyCook.RecipesAPI.Models;
using static TastyCook.Contracts.Contracts;

namespace TastyCook.RecipesAPI.Consumers;

public class UserUpdatedConsumer : IConsumer<UserItemUpdated>
{
    private readonly UserService _userService;

    public UserUpdatedConsumer(UserService userService)
    {
        _userService = userService;
    }

    public async Task Consume(ConsumeContext<UserItemUpdated> context)
    {
        var message = context.Message;
        await Console.Out.WriteLineAsync($"Message from Producer : {message.Email}");

        _userService.Update(new User { Id = message.Id, Email = message.Email, Password = message.Password });

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