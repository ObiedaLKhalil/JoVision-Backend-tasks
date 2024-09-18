using Microsoft.AspNetCore.Mvc;
using System;

namespace Task45.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BirthDateController : ControllerBase
    {
        // GET: api/birthdate
        [HttpGet]
        public IActionResult GetAge([FromQuery] string name, [FromQuery] int? years, [FromQuery] int? months, [FromQuery] int? days)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = "anonymous";
            }

            if (!years.HasValue || !months.HasValue || !days.HasValue)
            {
                return Ok($"Hello {name}, I can't calculate your age without knowing your birthdate!");
            }

            var birthdate = new DateTime(years.Value, months.Value, days.Value);
            var today = DateTime.Today;
           

            int age = today.Year - birthdate.Year;
            if (birthdate > today.AddYears(-age)) age--;

            return Ok($"Hello {name}, your age is {age}.");
        }
    }
}
