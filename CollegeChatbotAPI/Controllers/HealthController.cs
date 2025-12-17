using Microsoft.AspNetCore.Mvc;

namespace CollegeChatbotAPI.Controllers
{
    [ApiController]
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Check()
        {
            return Ok("API is running");
        }
    }
}
