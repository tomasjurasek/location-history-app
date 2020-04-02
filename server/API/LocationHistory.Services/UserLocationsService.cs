using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace LocationHistory.Services
{
    public class UserLocationsService
    {
        private readonly GoogleLocationParser googleLocationParser;
        private readonly AmazonService amazonService;
        private readonly ILogger<UserLocationsService> logger;
        private readonly AzureBlobLocationDataFileService azureBlobLocationDataFileService;

        public UserLocationsService(GoogleLocationParser googleLocationParser, AmazonService amazonService, ILogger<UserLocationsService> logger, AzureBlobLocationDataFileService azureBlobLocationDataFileService)
        {
            this.googleLocationParser = googleLocationParser;
            this.amazonService = amazonService;
            this.logger = logger;
            this.azureBlobLocationDataFileService = azureBlobLocationDataFileService;
        }

        public async Task CreateUserLocationsAsync(string userId, string phone, byte[] data)
        {
            logger.LogTrace($"Parsing data in {nameof(GoogleLocationParser)}.");
            var locations = googleLocationParser.Parse(data);

            await azureBlobLocationDataFileService.UploadCsvData(userId, phone, locations);

            logger.LogTrace($"Uploading data in {nameof(AmazonService)}.");
            //await amazonService.UploadCsvData(userId, phone, locations);

        }
    }
}
