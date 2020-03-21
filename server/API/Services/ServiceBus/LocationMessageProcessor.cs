using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using API.Services.ServiceBus;
using Database;
using Database.Entities;
using Microsoft.Extensions.Logging;

namespace Services.ServiceBus
{
    public class LocationMessageProcessor
    {
        private readonly ILogger<LocationMessageProcessor> logger;
        private readonly UserLocationsService userLocationsService;
        private readonly AzureBlobService azureBlobService;
        private readonly LocationDbContext locationDbContext;

        public LocationMessageProcessor(ILogger<LocationMessageProcessor> logger,
            UserLocationsService userLocationsService,
            AzureBlobService azureBlobService,
            LocationDbContext locationDbContext)
        {
            this.logger = logger;
            this.userLocationsService = userLocationsService;
            this.azureBlobService = azureBlobService;
            this.locationDbContext = locationDbContext;
        }

        public async Task ProcessAsync(LocationsCreatedMessage message, CancellationToken cancellationToken)
        {
            var userId = message.UserId;
            string folderPath = string.Empty;
            User user = null;
            try
            {
                using (var stream = await azureBlobService.DownloadFile(userId))
                {
                    if (stream != null)
                    {
                        stream.Position = 0;
                        var data = GetLocationHistoryDataFromZipStream(stream);

                        logger.LogInformation("Processing location data.");
                        await userLocationsService.CreateUserLocationsAsync(userId, data);

                        logger.LogInformation("Getting user info from DB.");
                        // TODO: change FirstOrDefault to SingleOrDefault and add unique index to UserIdentifier
                        user = locationDbContext.Users.FirstOrDefault(s => s.UserIdentifier == userId);
                        if (user != null)
                        {
                            user.Status = Status.Done;
                        }
                    }
                    else
                    {
                        logger.LogWarning("No data downloaded from Azure Blob Storage.");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Processing failed for user {UserId}.", userId);
                if (user != null)
                {
                    user.Status = Status.Failed;
                }
            }
            finally
            {
                if (!string.IsNullOrEmpty(folderPath))
                {
                    try
                    {
                        Directory.Delete(folderPath, true);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Deleting of directory {DirectoryName} failed for user {UserId}.", folderPath, userId);
                    }
                }

                logger.LogInformation("Deleting file from Azure Blob Storage for user {UserId}", userId);
                await azureBlobService.DeleteFile(userId);

                await locationDbContext.SaveChangesAsync();
            }
        }

        private byte[] GetLocationHistoryDataFromZipStream(Stream stream)
        {
            using (var archive = new ZipArchive(stream))
            {
                foreach (var entry in archive.Entries)
                {
                    logger.LogInformation("Zip archive entry: {ZipArchiveEntry}", entry.FullName);
                }

                var regexp = @"Takeout\/[^\/]+\/[^\/]+\.json";
                var locationHistoryEntry = archive.Entries.SingleOrDefault(entry =>
                    Regex.Match(entry.FullName, regexp, RegexOptions.IgnoreCase).Success);

                if (locationHistoryEntry == null)
                {
                    throw new Exception("JSON file with location history not found in zip archive.");
                }

                using (var entryStream = locationHistoryEntry.Open())
                {
                    using (var reader = new BinaryReader(entryStream))
                    {
                        return reader.ReadBytes((int) locationHistoryEntry.Length);
                    }
                }
            }
        }
    }
}