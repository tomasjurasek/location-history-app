using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class UserLocationsService
    {
        private readonly GoogleLocationParser googleLocationParser;
        private readonly AmazonService amazonService;

        public UserLocationsService(GoogleLocationParser googleLocationParser, AmazonService amazonService)
        {
            this.googleLocationParser = googleLocationParser;
            this.amazonService = amazonService;
        }

        public Task CreateUserLocationsAsync(string userId, string jsonFilePath)
        {
            var locations = googleLocationParser.Parse(jsonFilePath);
            return amazonService.UploadCsvData(userId, locations);
        }
    }
}
