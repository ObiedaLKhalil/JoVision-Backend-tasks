using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

namespace Task47.Controllers
{
    public class Metadata
    {
        public string Owner { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastModifiedTime { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ImageDeleteController : ControllerBase
    {
        private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");
        private readonly string _metadataPath = Path.Combine(Directory.GetCurrentDirectory(), "Metadata");

        [HttpGet]
        public IActionResult DeleteImage([FromQuery] string fileName="", [FromQuery] string fileOwner="")
        {
            // Check if fileName or fileOwner is missing
            if(fileName==""||fileOwner=="")
            {
                return BadRequest("File name or owner missing.");
            }

            // Construct full file paths for the image and metadata
            var filePath = Path.Combine(_uploadPath, fileName);
            var metadataFilePath = Path.Combine(_metadataPath, Path.ChangeExtension(fileName, ".json"));

            // Check if the file exists
            if (!System.IO.File.Exists(filePath))
            {
                return BadRequest("File does not exist.");
            }

            try
            {
                // Read and verify the owner in metadata
                var metadata = System.Text.Json.JsonSerializer.Deserialize<Metadata>(System.IO.File.ReadAllText(metadataFilePath));
                if (metadata.Owner != fileOwner)
                {
                    return Forbid("Owner does not match.");
                }

                // Delete the image and its metadata
                System.IO.File.Delete(filePath);
                System.IO.File.Delete(metadataFilePath);

                return Ok($"File {fileName} deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }
    }
}
