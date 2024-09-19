using Microsoft.AspNetCore.Mvc;

namespace Task46.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GreeterController : ControllerBase
    {
        // POST: api/greeter
        [HttpPost]
        public IActionResult Greet([FromForm] string name="")
        {
            if (name=="")
            {
                name = "anonymous";
            }

            return Ok($"Hello {name}");
        }
    }
}
