using Microsoft.AspNetCore.Mvc;

namespace TastyCook.ProductsAPI.Controllers
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