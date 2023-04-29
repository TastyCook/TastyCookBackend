using MassTransit;
using TastyCook.RecipesAPI;
using TastyCook.RecipesAPI.Entities;
using TastyCook.RecipesAPI.Models;
using static TastyCook.Contracts.Contracts;

namespace TastyCook.RecipesAPI.Consumers;

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
        await Console.Out.WriteLineAsync($"Message from Producer : {message.Email}");

        _userService.Add(new User { Id = message.Id, Email = message.Email, Password = message.Password });

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