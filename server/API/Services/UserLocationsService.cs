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

        public async Task<IEnumerable<Locations>> CreateUserLocationsAsync(string userId, string jsonFilePath)
        {
            var locations = googleLocationParser.Parse(jsonFilePath).ToList();
            await amazonService.UploadCsvData(userId, locations);
           
            return locations;
        }
    }
}
