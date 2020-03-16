using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [Route("api/users")]
    public partial class UsersController : Controller
    {
        private readonly UserLocationsService locationService;
        private readonly ILogger<UsersController> logger;

        public UsersController(UserLocationsService locationService, ILogger<UsersController> logger)
        {
            this.locationService = locationService;
            this.logger = logger;
        }

        [HttpPost("{userId}/file")]
        public async Task<UserLocationViewModel> UploadFileAsync(string userId, [FromForm] IFormFile file)
        {
            var response = new UserLocationViewModel();
            string tempDirectoryPath = null;

            try
            {
                tempDirectoryPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                Directory.CreateDirectory(tempDirectoryPath);

                var uploadedFilePath = Path.Combine(tempDirectoryPath, file.FileName);
                await using (Stream stream = new FileStream(uploadedFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var extractedDirectoryPath = Directory.CreateDirectory(Path.Combine(tempDirectoryPath, "data"));
                ZipFile.ExtractToDirectory(uploadedFilePath, extractedDirectoryPath.FullName);

                var jsonPath = Path.Combine(extractedDirectoryPath.FullName, "Takeout", "Location History",
                    "Location History.json");
                var jsonData = await System.IO.File.ReadAllTextAsync(jsonPath);
                var locations = await locationService.CreateUserLocationsAsync(userId, jsonData);
                response.Locations = locations.Select(s => new LocationViewModel
                {
                    DateTimeUtc = s.DateTimeUtc,
                    Accuracy = s.Accuracy,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude
                }).ToList();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(UsersController));
            }
            finally
            {
                try
                {
                    if (tempDirectoryPath != null)
                    {
                        Directory.Delete(tempDirectoryPath);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, nameof(UsersController));
                }
            }

            return response;
        }
    }
}