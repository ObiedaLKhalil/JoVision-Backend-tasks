using Microsoft.AspNetCore.Mvc;
using System;

namespace YourNamespace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BirthDateController : ControllerBase
    {
        // POST: api/birthdate
        [HttpPost]
        public IActionResult CalculateAge( [FromForm] int? years, [FromForm] int? months, [FromForm] int? days, [FromForm] string name="")
        {
            // If the name is not provided, default to "anonymous"
            if (name=="")
            {
                name = "anonymous";
            }

            // If any birthdate values are missing, return a message
            if (!years.HasValue || !months.HasValue || !days.HasValue)
            {
                return Ok($"Hello {name}, I can't calculate your age without knowing your birthdate!");
            }

            // Calculate the birthdate
            var birthdate = new DateTime(years.Value, months.Value, days.Value);
            var today = DateTime.Today;

            // Calculate the age
            int age = today.Year - birthdate.Year;
            if (birthdate > today.AddYears(-age)) age--; // Adjust if the birthday hasn't occurred this year

            return Ok($"Hello {name}, your age is {age}.");
        }
    }
}
