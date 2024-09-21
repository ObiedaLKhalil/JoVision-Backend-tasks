using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
namespace Task47.Controllers
{
    public class MetadataForRetrive
    {
        public string Owner { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastModifiedTime { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class RetrieveImageController : ControllerBase
    {
        private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");
        private readonly string _metadataPath = Path.Combine(Directory.GetCurrentDirectory(), "Metadata");

        [HttpGet]
        public IActionResult RetrieveImage([FromQuery] string fileName = "", [FromQuery] string fileOwner = "")
        {
            if (fileName==""||fileOwner=="")
            {
                return BadRequest("File name or owner is missing.");
            }
            var filePath = Path.Combine(_uploadPath, fileName);
            var metadataFilePath = Path.Combine(_metadataPath, Path.ChangeExtension(fileName, ".json"));
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }

            // Check if metadata exists
            if (!System.IO.File.Exists(metadataFilePath))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Metadata not found.");
            }
            try
            {
                // Read and verify the owner in metadata
                var metadata = System.Text.Json.JsonSerializer.Deserialize<MetadataForRetrive>(System.IO.File.ReadAllText(metadataFilePath));
                if (metadata.Owner != fileOwner)
                {
                    return Forbid("Owner does not match.");
                }
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                var fileExtension = Path.GetExtension(filePath).ToLower();

                // Return the file with appropriate MIME type
                var contentType = fileExtension switch
                {
                    ".jpg" => "image/jpeg",
                    ".png" => "image/png",
                    // Add other file types here if needed
                    _ => "application/octet-stream",
                };

                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }


        }
    }





        }