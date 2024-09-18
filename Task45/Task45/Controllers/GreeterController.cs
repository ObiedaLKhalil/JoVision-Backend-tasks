using Microsoft.AspNetCore.Mvc;

namespace MultiControllerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GreeterController : ControllerBase
    {
        // GET: api/greeter
        [HttpGet]
        public IActionResult Greet([FromQuery] string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = "anonymous";
            }

            var message = $"Hello {name}";
            return Ok(message);
        }
    }
}
