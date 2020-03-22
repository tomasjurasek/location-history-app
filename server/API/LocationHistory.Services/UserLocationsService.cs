using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace LocationHistory.Services
{
    public class UserLocationsService
    {
        private readonly GoogleLocationParser googleLocationParser;
        private readonly AmazonService amazonService;
        private readonly ILogger<UserLocationsService> logger;

        public UserLocationsService(GoogleLocationParser googleLocationParser, AmazonService amazonService, ILogger<UserLocationsService> logger)
        {
            this.googleLocationParser = googleLocationParser;
            this.amazonService = amazonService;
            this.logger = logger;
        }

        public Task CreateUserLocationsAsync(string userId, string phone, byte[] data)
        {
            logger.LogTrace($"Parsing data in {nameof(GoogleLocationParser)}.");
            var locations = googleLocationParser.Parse(data);

            logger.LogTrace($"Uploading data in {nameof(AmazonService)}.");
            return amazonService.UploadCsvData(userId, phone, locations);
        }
    }
}
