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

        public Task CreateUserLocationsAsync(string userId, byte[] data)
        {
            var locations = googleLocationParser.Parse(data);
            return amazonService.UploadCsvData(userId, locations);
        }
    }
}
