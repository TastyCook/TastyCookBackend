using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TastyCook.UsersAPI.Entities;

namespace TastyCook.UsersAPI
{
    public class SeedHelper
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetService<UserContext>();
            var roles = new[] { "Admin", "User" };

            foreach (string role in roles)
            {
                var roleStore = new RoleStore<IdentityRole>(context);

                if (!context.Roles.Any(r => r.Name == role))
                {
                    await roleStore.CreateAsync(new IdentityRole(role));
                }
            }

            var user = new User
            {
                Email = "tastycookadmin@nure.ua",
                NormalizedEmail = "TASTYCOOKADMIN@NURE.UA",
                UserName = "TastyCookAdmin",
                NormalizedUserName = "TASTYCOOKADMIN",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D")
            };


            if (!context.Users.Any(u => u.UserName == user.UserName))
            {
                var password = new PasswordHasher<User>();
                var hashed = password.HashPassword(user, "qweqwe");
                user.PasswordHash = hashed;

                var userStore = new UserStore<User>(context);
                var result = await userStore.CreateAsync(user);

            }

            await AssignRoles(serviceProvider, user.Email, roles);

            await context.SaveChangesAsync();
        }

        public static async Task<IdentityResult> AssignRoles(IServiceProvider services, string email, string[] roles)
        {
            UserManager<User> _userManager = services.GetService<UserManager<User>>();
            User user = await _userManager.FindByEmailAsync(email);
            var result = await _userManager.AddToRolesAsync(user, roles);

            return result;
        }

    }
}