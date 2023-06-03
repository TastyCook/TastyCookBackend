using MassTransit;
using TastyCook.RecipesAPI.Entities;
using TastyCook.RecipesAPI.Models;
using TastyCook.RecipesAPI.Services;
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

        var item = _userService.GetById(message.Id);

        if (item is not null)
        {
            return;
        }

        _userService.Add(new User { Id = message.Id, Email = message.Email, Password = message.Password, UserName = message.Username });
    }
}