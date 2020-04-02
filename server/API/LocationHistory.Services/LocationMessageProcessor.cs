using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using LocationHistory.Database;
using LocationHistory.Database.Entities;
using LocationHistory.Services.ServiceBus;
using Microsoft.Extensions.Logging;

namespace LocationHistory.Services
{
    public class LocationMessageProcessor
    {
        private readonly ILogger<LocationMessageProcessor> logger;
        private readonly UserLocationsService userLocationsService;
        private readonly AzureBlobLocationFileService azureBlobService;
        private readonly LocationDbContext locationDbContext;

        public LocationMessageProcessor(ILogger<LocationMessageProcessor> logger,
            UserLocationsService userLocationsService,
            AzureBlobLocationFileService azureBlobService,
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
            User user = locationDbContext.Users.FirstOrDefault(s => s.UserIdentifier == userId);
            try
            {
                if (user != null)
                {
                    logger.LogInformation("Downloading file from Azure Blob Storage for user {UserId}.", userId);
                    using (var stream = await azureBlobService.DownloadFile(userId))
                    {
                        if (stream != null)
                        {
                            stream.Position = 0;
                            var data = GetLocationHistoryDataFromZipStream(stream);

                            logger.LogInformation("Processing location data.");
                            await userLocationsService.CreateUserLocationsAsync(user.UserIdentifier, user.Phone, data);

                            logger.LogInformation("Getting user info from DB.");
                            user.Status = Status.Done;
                        }
                        else
                        {
                            logger.LogWarning("No data downloaded from Azure Blob Storage.");
                        }
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
                if (user != null)
                {
                    user.Phone = null;
                    user.VerifyCode = null;
                }

                logger.LogInformation("Deleting file from Azure Blob Storage for user {UserId}", userId);
                await azureBlobService.DeleteFile(userId);

                logger.LogInformation("Saving user info int DB for user {UserId}", userId);
              
                await locationDbContext.SaveChangesAsync(cancellationToken);
            }
        }

        private byte[] GetLocationHistoryDataFromZipStream(Stream stream)
        {
            using (var archive = new ZipArchive(stream))
            {
                foreach (var entry in archive.Entries)
                {
                    logger.LogTrace("Zip archive entry: {ZipArchiveEntry}", entry.FullName);
                }

                var regexp = @"Takeout\/[^\/]+\/[^\/]+\.json";
                var locationHistoryEntry = archive.Entries.SingleOrDefault(entry =>
                    Regex.Match(entry.FullName, regexp, RegexOptions.IgnoreCase).Success);

                if (locationHistoryEntry == null)
                {
                    throw new Exception("JSON file with location history not found in zip archive.");
                }

                logger.LogInformation("Uncompressing zip archive entry: {ZipArchiveEntry}", locationHistoryEntry.FullName);

                using (var entryStream = locationHistoryEntry.Open())
                {
                    using (var reader = new BinaryReader(entryStream))
                    {
                        return reader.ReadBytes((int)locationHistoryEntry.Length);
                    }
                }
            }
        }
    }
}