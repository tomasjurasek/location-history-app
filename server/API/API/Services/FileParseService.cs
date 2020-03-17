using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace API.Services
{
    public class FileParseService : BackgroundService
    {
        private readonly UserLocationsService userLocationsService;

        public FileParseService(UserLocationsService userLocationsService)
        {
            this.userLocationsService = userLocationsService;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var folders = Directory.GetDirectories(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));
                foreach (var folder in folders)
                {
                    var file = Directory.GetFiles(folder).FirstOrDefault();
                    var extractedDirectoryPath = Directory.CreateDirectory(Path.Combine(folder, "data"));
                    ZipFile.ExtractToDirectory(file, extractedDirectoryPath.FullName);

                    var jsonData = await GetJsonData(extractedDirectoryPath.FullName);
                    var locations = await userLocationsService.CreateUserLocationsAsync(Guid.NewGuid().ToString(), jsonData);
                }

                await Task.Delay(10000);
            }
        }

        private async Task<string> GetJsonData(string directoryPath)
        {
            var jsonPathEn = Path.Combine(directoryPath, "Takeout", "Location History", "Location History.json");
            var jsonPathCz = Path.Combine(directoryPath, "Takeout", "Historie polohy", "Historie polohy.json");
            string finalJsonPath = null;

            if (System.IO.File.Exists(jsonPathEn))
            {
                finalJsonPath = jsonPathEn;
            }
            else if (System.IO.File.Exists(jsonPathCz))
            {
                finalJsonPath = jsonPathCz;
            }
           

            var jsonData = await System.IO.File.ReadAllTextAsync(finalJsonPath);
            return jsonData;
        }
    }
}
