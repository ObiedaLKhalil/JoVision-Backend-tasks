using Microsoft.AspNetCore.Mvc;

namespace Task44.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GreeterController : ControllerBase
    {
        // GET: api/greeter
        [HttpGet]
        public IActionResult Greet([FromQuery] string name="")
        {
            // Check if the 'name' parameter is null or empty
            if (name=="")
            {
                name = "anonymous";
            }

            // Create the greeting message
            var message = $"Hello {name}";

            // Return the message with a 200 OK status
            return Ok(message);
        }
    }
}
