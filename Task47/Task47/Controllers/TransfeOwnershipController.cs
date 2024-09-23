using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;
namespace Task47.Controllers {
    public class MetadataForTransfareOwnership
    {
        public string Owner { get; set; } = "";
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public DateTime LastModifiedTime { get; set; } = DateTime.Now;
    }
    [Route("api/[controller]")]
    [ApiController]
    public class TransfeOwnershipController : ControllerBase
    {
        private readonly string _metadataPath = Path.Combine(Directory.GetCurrentDirectory(), "Metadata");
        [HttpGet]
        public IActionResult TransferOwnership([FromQuery] string oldOwner="", [FromQuery] string newOwner="")
        {
            if (oldOwner==""||newOwner=="")
            {
                return BadRequest("oldOwner name or newOwner name is missing.");
            }
            try {
                var metadataFiles = Directory.GetFiles(_metadataPath, "*.json");
                List<object> newOwnerFiles = new List<object>();
                for (int i = 0; i<metadataFiles.Length; i++)
                {
                    var metadata = JsonSerializer.Deserialize<MetadataForTransfareOwnership>(System.IO.File.ReadAllText(metadataFiles[i]));
                    string fileName = Path.GetFileNameWithoutExtension(metadataFiles[i]);
                    if (metadata.Owner == oldOwner)
                    {
                        metadata.Owner = newOwner;

                        // Save the updated metadata back to the file
                        var updatedMetadataJson = JsonSerializer.Serialize(metadata);
                        System.IO.File.WriteAllText(metadataFiles[i], updatedMetadataJson);
                    }
                    if (metadata.Owner == newOwner)
                    {
                        newOwnerFiles.Add(new { fileName, newOwner, oldOwner });
                    }
                }

                return Ok(newOwnerFiles);
            } 


            catch(Exception ex) 
            { return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}"); }

        }
    }


    }



