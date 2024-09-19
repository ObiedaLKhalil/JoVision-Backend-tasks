using Microsoft.AspNetCore.Http;
// This provides functionality for handling HTTP requests and responses, including handling files uploaded through a form.
// It includes types that work with files, headers, cookies, and more.
//IFormFile: Represents a file sent with an HTTP request (used for uploading images).
//StatusCodes: Provides constants for various HTTP status codes like Status200OK or Status500InternalServerError.
using Microsoft.AspNetCore.Mvc;
//This library is essential for building APIs in ASP.NET Core.
//It contains types like ControllerBase, IActionResult, and routing attributes,
//which are used to create web APIs and handle HTTP requests (GET, POST, etc.).
//IActionResult: Represents the result of an action method, used to return HTTP responses (e.g., Ok(), BadRequest(), Created()).
using System;
using System.IO;
//functionality for working with the file system, allowing you to read from and write to files, directories, and streams.
//FileStream: Used to read/write files(e.g., saving the uploaded image to disk).
//Directory: Used to create and check if directories exist (e.g., creating the "UploadedFiles" and "Metadata" folders).
//Path: Provides methods for manipulating file paths (e.g., Path.Combine() to combine folder and file names).
using System.Threading.Tasks;
//handle asynchronous programming. When dealing with file uploads and I/O operations, asynchronous methods (async and await) are used to prevent blocking the main thread.
//Task<IActionResult>: Represents an asynchronous operation that returns an HTTP response.
//await file.CopyToAsync(): Asynchronously copy the uploaded file to a stream.
using System.Text.Json;
using System.Resources;
//serializing and deserializing JSON data. You'll need it to save metadata (like the image's owner and creation date) as a JSON file.
//JsonSerializer.Serialize(): Converts C# objects to JSON strings (used to store the metadata of the uploaded image as a JSON file).
namespace Task47.Controllers {
    //routes determine how incoming HTTP requests map to controller methods.
    // static part of the URL and indicates that the route will start with api/.
    //This is a placeholder that gets replaced by the name of the controller when the request is routed.

    [Route("api/[controller]")]
    //his attribute marks a class as an API controller. It introduces several conventions that make building Web APIs easier and more efficient.
    // The framework automatically tries to bind the incoming request to action parameters. For example, it will bind form data, query parameters, and request bodies to method parameters.
    //If model validation fails (e.g., a required field is missing), the API will automatically return a 400 Bad Request response without you having to manually check the model state.

    [ApiController]
    public class ImageUploadController : ControllerBase
    {
        //variable can only be assigned during its declaration or in the constructor of the class. After that, it cannot be changed, ensuring that the paths remain constant throughout the lifetime of the class instance.
        // This method retrieves the current working directory of the application. It gives you the path where the application is running.
        private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");
        private readonly string _metadataPath = Path.Combine(Directory.GetCurrentDirectory(), "Metadata");

        public ImageUploadController()
        {
            // Ensure directories exist for storing files and metadata
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
               
            }
            if (!Directory.Exists(_metadataPath))
            {
                Directory.CreateDirectory(_metadataPath);
            }
        }
        //_uploadPath=C:\Users\obied\OneDrive\Desktop\JoVision-Backend-tasks\Task47\Task47\UploadedFiles
        //_metadataPath=C:\Users\obied\OneDrive\Desktop\JoVision-Backend-tasks\Task47\Task47\Metadata

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file, [FromForm] string owner)
        {
            // Check if file is provided
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
             
            // Ensure file is a .jpg
            if (!file.FileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only JPG files are allowed.");
            }

            // Get the filename and full path to store the file
            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(_uploadPath, fileName);
            //fileName=RGBimage.jpg
            //(file.FileName=RGBimage.jpg
            //filePath=C:\Users\obied\OneDrive\Desktop\JoVision-Backend-tasks\Task47\Task47\UploadedFiles\RGBimage.jpg
            
            // Check if the file already exists
            if (System.IO.File.Exists(filePath))
            {
                return BadRequest("File with the same name already exists.");
            }
           
            try
            {
                // Save the image file
                //File is Copied: The contents of the uploaded file are copied from the IFormFile object to the FileStream. This process is asynchronous, meaning it doesn't block the thread while the file is being written to disk.
                //Stream is Closed: Because you are using a using statement, once the block is exited, the FileStream will be automatically closed and disposed of.This frees up any resources associated with the stream.

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
               
                // Save metadata (owner, creation time, last modified time)
                var metadata = new
                {
                    Owner = owner,
                    CreationTime = DateTime.UtcNow,
                    LastModifiedTime = DateTime.UtcNow
                };
                //C:\Users\obied\OneDrive\Desktop\JoVision-Backend-tasks\Task47\Task47\Metadata\RGBimage.json
                var metadataFilePath = Path.Combine(_metadataPath, Path.ChangeExtension(fileName, ".json"));
                //{"Owner":"Obieda","CreationTime":"2024-09-19T08:30:28.6975952Z","LastModifiedTime":"2024-09-19T08:30:28.6975958Z"}
                System.IO.File.WriteAllText(metadataFilePath, System.Text.Json.JsonSerializer.Serialize(metadata));

                // Return success response
                return Created($"api/imageupload/{fileName}", new { Message = "File uploaded successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }
    }
}