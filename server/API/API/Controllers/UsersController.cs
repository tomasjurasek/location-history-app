using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API.Controllers
{
    [Route("api/[controller]")]
    public partial class UsersController : Controller
    {

        [HttpGet("{userId}/locations")]
        public IEnumerable<Locations> GetAsync(string userId)
        {
            return new Locations[] {
                new Locations{
                    timestampMs = "1517645260330",
                    latitudeE7 = 500437725,
                    longitudeE7 = 144549068,
                    accuracy = 96
                },
                new Locations{
                    timestampMs = "1517649982844",
                    latitudeE7 = 500437275,
                    longitudeE7 = 144545330,
                    accuracy = 33
                }
            };
        }

        [HttpPost]
        public async Task<string> CreateUser(CreateUser model)
        {
            return "";
        }

        [HttpPost("{userId}/file")]
        public async Task<IActionResult> UploadFileAsync(string userId, [FromForm]IFormFile file)
        {
            var userFolderPath = $"{Directory.GetCurrentDirectory()}/wwwroot/{userId}";
            if (!System.IO.File.Exists(userFolderPath))
            {
                Directory.CreateDirectory(userFolderPath);

                var path = Path.Combine(userFolderPath, file.FileName);
                using (System.IO.Stream stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var userFolderDataPath = Directory.CreateDirectory(Path.Combine(userFolderPath, "data"));
                ZipFile.ExtractToDirectory(path, userFolderDataPath.FullName);
                string jsonPath = Path.Combine(userFolderDataPath.FullName, "Takeout", "Location History", "Location History.json");

                var json = System.IO.File.ReadAllText(jsonPath);
                var jsonData = JsonConvert.DeserializeObject<GoogleRootObject>(json); 
                //TODO Filter data and save 

                System.IO.File.Delete(userFolderPath);
            }

            return Ok();
        }
    }
}
