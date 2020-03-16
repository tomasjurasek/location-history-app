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
using Newtonsoft.Json;

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

        [HttpGet("locations")]
        public Task<List<UserLocationViewModel>> GetUsers()
        {
            var response = new List<UserLocationViewModel>();
            var usersLocations = locationService.GetUserLocations();

            foreach (var userLocations in usersLocations)
            {
                response.Add(new UserLocationViewModel
                {
                    Id = userLocations.RowKey,
                    Locations = JsonConvert.DeserializeObject<List<LocationViewModel>>(userLocations?.JsonLocations)
                });
                
            }

            return Task.FromResult(response);
        }

        [HttpGet("{userId}/locations")]
        public Task<IEnumerable<LocationViewModel>> Get(string userId)
        {
            var locations = locationService.GetUserLocations(userId);

            var result = locations.Select(s => new LocationViewModel
            {
                DateTimeUtc = s.DateTimeUtc,
                Accuracy = s.Accuracy,
                Latitude = s.Latitude,
                Longitude = s.Longitude
            });

            return Task.FromResult(result);
        }

        [HttpPost("{userId}/file")]
        public async Task<IActionResult> UploadFileAsync(string userId, [FromForm]IFormFile file)
        {
            try
            {
                var userFolderPath = $"{Directory.GetCurrentDirectory()}/wwwroot/{userId}";
                if (!System.IO.File.Exists(userFolderPath))
                {
                    Directory.CreateDirectory(userFolderPath);

                    var filePath = Path.Combine(userFolderPath, file.FileName);
                    using (System.IO.Stream stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var userFolderDataPath = Directory.CreateDirectory(Path.Combine(userFolderPath, "data"));
                    ZipFile.ExtractToDirectory(filePath, userFolderDataPath.FullName);

                    var jsonPath = Path.Combine(userFolderDataPath.FullName, "Takeout", "Location History", "Location History.json");
                    var jsonData = await System.IO.File.ReadAllTextAsync(jsonPath);
                    await locationService.CreateUserLocationsAsync(userId, jsonData);

                    System.IO.File.Delete(userFolderPath);
                }
            }
            catch (System.Exception ex)
            {
                logger.LogError(ex, nameof(UsersController));
            }

            return Ok();
        }
    }
}
