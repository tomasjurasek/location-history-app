using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [Route("api/users")]
    public partial class UsersController : Controller
    {
        private readonly UserLocationsService locationService;
        private readonly ILogger<UsersController> logger;
        private readonly IWebHostEnvironment env;
        private readonly IConfiguration config;

        public UsersController(UserLocationsService locationService, ILogger<UsersController> logger, IWebHostEnvironment env, IConfiguration config)
        {
            this.locationService = locationService;
            this.logger = logger;
            this.env = env;
            this.config = config;
        }

        [HttpPost("{userId}/file")]
        [RequestSizeLimit(104857600)]
        //[RequestFormLimits(MultipartBodyLengthLimit = 104857600)]
        public async Task<ActionResult<UserLocationViewModel>> UploadFileAsync(string userId, [FromForm] IFormFile file)
        {
            var response = new UserLocationViewModel
            {
                Id = userId
            };
            string tempDirectoryPath = null;

            if (file == null)
            {
                return BadRequest("No file has been uploaded.");
            }

            //if(!IsFileContentTypeValid(file))
            //{
            //    return BadRequest("File must be a zip.");
            //}

            if (!IsFileLengthValid(file))
            {
                return BadRequest("File size exceeded configured limit.");
            }

            try
            {
                //var userFolderPath = $"{Directory.GetCurrentDirectory()}/wwwroot/{userId}";
                tempDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot", $"{userId}");
                Directory.CreateDirectory(tempDirectoryPath);

                var uploadedFilePath = Path.Combine(tempDirectoryPath, file.FileName);
                await using (Stream stream = new FileStream(uploadedFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                //var extractedDirectoryPath = Directory.CreateDirectory(Path.Combine(tempDirectoryPath, "data"));
                //ZipFile.ExtractToDirectory(uploadedFilePath, extractedDirectoryPath.FullName);

                //var jsonData = await GetJsonData(extractedDirectoryPath.FullName);

                //var locations = await locationService.CreateUserLocationsAsync(userId, jsonData);
                //response.Locations = locations.Select(s => new LocationViewModel
                //{
                //    DateTimeUtc = s.DateTimeUtc,
                //    Accuracy = s.Accuracy,
                //    Latitude = s.Latitude,
                //    Longitude = s.Longitude
                //}).ToList();
            }
            catch (Exception ex)
            {
                //throw new Exception("Processing of uploaded file failed.", ex);
                logger.LogError(ex, "Processing of uploaded file failed.");
            }
            finally
            {
                //try
                //{
                //    if (tempDirectoryPath != null)
                //    {
                //        Directory.Delete(tempDirectoryPath, true);
                //    }
                //}
                //catch (Exception ex)
                //{
                //    // throw new Exception($"Deleting of temporary folder '{tempDirectoryPath}' failed.", ex);
                //    logger.LogError(ex, $"Deleting of temporary folder '{tempDirectoryPath}' failed.");
                //}
            }

            return response;
        }

        private bool IsFileLengthValid(IFormFile file)
        {
            var maxMaxUploadedFileLength = config.GetValue<long>("MaxUploadedFileLength");
            if (maxMaxUploadedFileLength == default)
            {
                throw new Exception("Missing configuration of 'MaxUploadedFileLength'.");
            }


            return file.Length <= maxMaxUploadedFileLength;
        }

        private bool IsFileContentTypeValid(IFormFile file)
        {
            return file.ContentType == "application/zip";
        }

        private async Task<string> GetJsonData(string directoryPath)
        {
            var jsonPathEn = Path.Combine(directoryPath, "Takeout", "Location History", "Location History.json");
            var jsonPathCz = Path.Combine(directoryPath, "Takeout", "Historie polohy", "Historie polohy.json");
            string finalJsonPath;

            if (System.IO.File.Exists(jsonPathEn))
            {
                finalJsonPath = jsonPathEn;
            }
            else if (System.IO.File.Exists(jsonPathCz))
            {
                finalJsonPath = jsonPathCz;
            }
            else
            {
                if (!TryGetSingleJsonFile(directoryPath, out finalJsonPath))
                {
                    throw new Exception($"JSON file with location history not found in '{directoryPath}'.");
                }
            }

            var jsonData = await System.IO.File.ReadAllTextAsync(finalJsonPath);
            return jsonData;
        }

        private bool TryGetSingleJsonFile(string directoryPath, out string jsonFilePath)
        {
            var dir = new DirectoryInfo(directoryPath);
            var files = dir.EnumerateFiles("*.json", SearchOption.TopDirectoryOnly).ToList();
            if (files.Count == 1)
            {
                jsonFilePath = files.Single().FullName;
                return true;
            }

            if (files.Count > 1)
            {
                jsonFilePath = default;
                return false;
            }

            foreach (var subdir in dir.EnumerateDirectories())
            {
                if (TryGetSingleJsonFile(subdir.FullName, out jsonFilePath))
                {
                    return true;
                }
            }

            jsonFilePath = default;
            return false;
        }
    }
}