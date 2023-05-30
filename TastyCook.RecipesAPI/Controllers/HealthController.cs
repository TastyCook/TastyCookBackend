using Microsoft.AspNetCore.Mvc;

<<<<<<< HEAD:TastyCook.UsersAPI/Controllers/HealthController.cs
namespace TastyCook.UsersAPI.Controllers
=======
namespace TastyCook.RecipesAPI.Controllers
>>>>>>> main:TastyCook.RecipesAPI/Controllers/HealthController.cs
{
    [ApiController]
    [Route("/health")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public string HealthPing()
        {
            return "Server is working";
        }
    }
}