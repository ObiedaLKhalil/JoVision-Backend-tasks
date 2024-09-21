using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;
using System.Resources;
namespace Task47.Controllers {
    public class MetadataForUpdate
    {
        public string Owner { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastModifiedTime { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class ImageUpdateController : ControllerBase {
        private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");
        
        private readonly string _metadataPath = Path.Combine(Directory.GetCurrentDirectory(), "Metadata");

        [HttpPost]
        public async Task<IActionResult> UpdateImage(IFormFile file, [FromForm] string owner)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Ensure file is a .jpg
            if (!file.FileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only JPG files are allowed.");
            }

            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(_uploadPath, fileName);
            var metadataFilePath = Path.Combine(_metadataPath, Path.ChangeExtension(fileName, ".json"));

            if (!System.IO.File.Exists(filePath) || !System.IO.File.Exists(metadataFilePath))
            {
                return BadRequest("File does not exist.");
            }
            var metadataJson = System.IO.File.ReadAllText(metadataFilePath);
            var metadata = System.Text.Json.JsonSerializer.Deserialize<MetadataForUpdate>(metadataJson);
            if (metadata.Owner != owner)
            {
                return Forbid("Owner does not match.");
            }

            try {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Update metadata with new last modified time
                metadata.LastModifiedTime = DateTime.UtcNow;
                System.IO.File.WriteAllText(metadataFilePath, System.Text.Json.JsonSerializer.Serialize(metadata));

                return Ok($"File {fileName} updated successfully.");
            }
            catch(Exception ex) { return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}"); }
        }

    }





}