using Microsoft.AspNetCore.Mvc;

namespace TastyCook.RecepiesAPI.Controllers
{
    [ApiController]
    [Route("/health")]
    public class HealthController : ControllerBase
    {
        [HttpGet("Test")]
        public string HealthPing()
        {
            return "Server is working";
        }
    }
}