using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;
using System.Resources;
using System.Collections.Generic;
using System.Linq;

namespace Task47.Controllers
{
    public enum FilterType
    {
        ByModificationDate,
        ByCreationDateDescending,
        ByCreationDateAscending,
        ByOwner
    }
    public class MetadataForFilter
    {
        public string Owner { get; set; } = "";
        public DateTime CreationTime { get; set; }= DateTime.Now;
        public DateTime LastModifiedTime { get; set; } = DateTime.Now;
    }

    [Route("api/[controller]")]
    [ApiController]
    public class FileFilterController : ControllerBase
    {
        private readonly string _metadataPath = Path.Combine(Directory.GetCurrentDirectory(), "Metadata");
        [HttpPost]
        public IActionResult FilterFiles([FromForm] DateTime creationDate , [FromForm] DateTime modificationDate, [FromForm] string owner , [FromForm] FilterType filterType )
        {
            if (filterType==null)
            {
                return BadRequest("FilterType is required.");
            }
            try 
            {
               
                var metadataFiles = Directory.GetFiles(_metadataPath, "*.json");
                List<object> filteredFiles = new List<object>();
                
                for (int i = 0; i<metadataFiles.Length; i++) {
                    var metadata = JsonSerializer.Deserialize<MetadataForFilter>(System.IO.File.ReadAllText(metadataFiles[i]));
                    DateTime creationTime = metadata.CreationTime;
                    DateTime lastModifiedTime = metadata.LastModifiedTime;
                    string _owner = metadata.Owner;
                    string fileName = Path.GetFileNameWithoutExtension(metadataFiles[i]);
                    
                    switch (filterType) {
                        case FilterType.ByModificationDate:
                            
                            {
                                if ((lastModifiedTime < modificationDate)&& modificationDate !=null) {
                                    filteredFiles.Add(new { fileName, _owner });
                                }

                                break;
                            }
                        case FilterType.ByCreationDateDescending:
                            {
                                if (creationDate !=null && (creationTime >creationDate))
                                {
                                    filteredFiles.Add(new { fileName, _owner });
                                    

                                }
                                break;
                            }
                        case FilterType.ByCreationDateAscending:
                            {
                                if (creationDate !=null && (creationTime >creationDate))
                                {
                                    filteredFiles.Add(new { fileName, _owner });
                                   

                                }
                                break;
                            }
                        case FilterType.ByOwner:
                            {
                                 if (owner !="" && _owner == owner)
                            {
                                filteredFiles.Add(new { fileName, _owner });
                            }
                            break;
                            }
                        default:
                            return BadRequest("Invalid FilterType.");


                    }

                  
                }
                if (filterType == FilterType.ByCreationDateDescending)
                {
                   
                }
                else if (filterType == FilterType.ByCreationDateAscending)
                {
                    
                }
                if (filterType == FilterType.ByCreationDateDescending)
                {
                    // Sort files by CreationTime in descending order
                    filteredFiles = filteredFiles.OrderByDescending(f =>
                    {
                        // Retrieve the file name from the filtered list
                        var fileName = ((dynamic)f).fileName;

                        // Read the metadata file for this specific file
                        var metadata = JsonSerializer.Deserialize<MetadataForFilter>(System.IO.File.ReadAllText(Path.Combine(_metadataPath, $"{fileName}.json")));

                        // Return the CreationTime to be used for sorting
                        return metadata.CreationTime;
                    }).ToList();
                }
                else if (filterType == FilterType.ByCreationDateAscending)
                {
                    // Sort files by CreationTime in ascending order
                    filteredFiles = filteredFiles.OrderBy(f =>
                    {
                        // Retrieve the file name from the filtered list
                        var fileName = ((dynamic)f).fileName;

                        // Read the metadata file for this specific file
                        var metadata = JsonSerializer.Deserialize<MetadataForFilter>(System.IO.File.ReadAllText(Path.Combine(_metadataPath, $"{fileName}.json")));

                        // Return the CreationTime to be used for sorting
                        return metadata.CreationTime;
                    }).ToList();
                }


                return Ok(filteredFiles);
            } 
            catch(Exception ex) {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}"); }

        }
    }

    }