using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services;
using Services.Options;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace API.Services.ServiceBus
{
    public class LocationCreatedReceiver : ServiceBusReceiver
    {
        private readonly ILogger<ServiceBusReceiver> logger;
        private readonly UserLocationsService userLocationsService;
        private readonly AzureBlobService azureBlobService;

        public LocationCreatedReceiver(IOptions<AzureServiceBusOptions> options, ILogger<ServiceBusReceiver> logger,
            UserLocationsService userLocationsService, AzureBlobService azureBlobService) : base(options, logger)
        {
            this.logger = logger;
            this.userLocationsService = userLocationsService;
            this.azureBlobService = azureBlobService;
        }

        public override async Task ProcessEventAsync(LocationsCreatedMessage message, CancellationToken cancellationToken)
        {
            var userId = message.UserId;
            try
            {
                using (var stream = await azureBlobService.DownloadFile(userId))
                {
                    if (stream != null)
                    {
                        stream.Position = 0;
                        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), userId);
                        Directory.CreateDirectory(Path.Combine(folderPath));
                        var uploadedFilePath = Path.Combine(folderPath, Path.GetRandomFileName());
                        using (Stream fileStream = new FileStream(uploadedFilePath, FileMode.Create))
                        {
                            await stream.CopyToAsync(fileStream);
                        }

                        var extractedDirectoryPath = Directory.CreateDirectory(Path.Combine(folderPath, "data"));
                        ZipFile.ExtractToDirectory(uploadedFilePath, extractedDirectoryPath.FullName);

                        var jsonData = GetJsonFilePath(extractedDirectoryPath.FullName);
                        await userLocationsService.CreateUserLocationsAsync(userId, jsonData);

                        Directory.Delete(folderPath, true);
                        await azureBlobService.DeleteFile(userId);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Processing failed for user - {userId}");
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
