using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace API.Services
{
    public class FileParseBackgroundService : BackgroundService
    {
        private readonly UserLocationsService userLocationsService;
        private readonly ILogger<FileParseBackgroundService> logger;

        public FileParseBackgroundService(UserLocationsService userLocationsService, ILogger<FileParseBackgroundService> logger)
        {
            this.userLocationsService = userLocationsService;
            this.logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(10000);
                try
                {
                    var folders = Directory.GetDirectories(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));
                    foreach (var folder in folders)
                    {
                        var file = Directory.GetFiles(folder).FirstOrDefault();
                        var extractedDirectoryPath = Directory.CreateDirectory(Path.Combine(folder, "data"));
                        ZipFile.ExtractToDirectory(file, extractedDirectoryPath.FullName);

                        var jsonData = GetJsonFilePath(extractedDirectoryPath.FullName);
                        var userId = new DirectoryInfo(folder).Name;
                        await userLocationsService.CreateUserLocationsAsync(userId, jsonData);

                        Directory.Delete(folder, true);
                    }

                }
                catch (Exception ex)
                {
                    logger.LogError(ex, nameof(FileParseBackgroundService));
                }
                
                await Task.Delay(10000);
            }
        }

        private string GetJsonFilePath(string directoryPath)
        {
            var jsonPathEn = Path.Combine(directoryPath, "Takeout", "Location History", "Location History.json");
            var jsonPathCz = Path.Combine(directoryPath, "Takeout", "Historie polohy", "Historie polohy.json");

            if (System.IO.File.Exists(jsonPathEn))
            {
                return jsonPathEn;
            }

            if (System.IO.File.Exists(jsonPathCz))
            {
                return jsonPathCz;
            }

            if (!TryGetSingleJsonFile(directoryPath, out var finalJsonPath))
            {
                throw new Exception($"JSON file with location history not found in '{directoryPath}'.");
            }

            return finalJsonPath;
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
