using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using LocationHistory.Database;
using LocationHistory.Services.BlobStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LocationHistory.Services
{
    public class UserLocationsService
    {
        private readonly GoogleLocationParser googleLocationParser;
        private readonly ILogger<UserLocationsService> logger;
        private readonly AzureBlobLocationDataFileService azureBlobLocationDataFileService;
        private readonly AzureBlobLocationFileService azureBlobLocationFileService;
        private readonly LocationDbContext locationDbContext;

        public UserLocationsService(GoogleLocationParser googleLocationParser,
            ILogger<UserLocationsService> logger,
            AzureBlobLocationDataFileService azureBlobLocationDataFileService,
            AzureBlobLocationFileService azureBlobLocationFileService,
            LocationDbContext locationDbContext)
        {
            this.googleLocationParser = googleLocationParser;
            this.logger = logger;
            this.azureBlobLocationDataFileService = azureBlobLocationDataFileService;
            this.azureBlobLocationFileService = azureBlobLocationFileService;
            this.locationDbContext = locationDbContext;
        }

        public async Task CreateUserLocationsAsync(string userId, string phone, byte[] data)
        {
            var locations = googleLocationParser.Parse(data);
            await azureBlobLocationDataFileService.UploadCsvData(userId, phone, locations);
        }

        public async Task<List<Locations>> GetLocations(string userId)
        {
            var locations = new List<Locations>();
            var stream = await azureBlobLocationDataFileService.Download(userId);
            using (var reader = new StreamReader(stream))
            {
                var line = await reader.ReadLineAsync();
                while (line != null)
                {
                    string[] values = line.Split(',');
                    if (values[0] != "id")
                    {
                        locations.Add(new Locations
                        {
                            DateTimeUtc = DateTime.ParseExact(values[1], "dd/MM/yyyy H:mm", CultureInfo.InvariantCulture),
                            Longitude = int.Parse(values[2]),
                            Latitude = int.Parse(values[3]),
                            Accuracy = int.Parse(values[4])
                        });

                    }
                    line = await reader.ReadLineAsync();
                }
            }

            return locations;
        }


        public async Task<bool> DeleteUserData(string userId)
        {
            var isLocationDataFileDeleted = await azureBlobLocationDataFileService.Delete(userId);
            var isLocationFoleDeleted = await azureBlobLocationFileService.Delete(userId);

            if (isLocationDataFileDeleted && isLocationFoleDeleted)
            {
                return true;
            }

            return false;
        }
    }
}
