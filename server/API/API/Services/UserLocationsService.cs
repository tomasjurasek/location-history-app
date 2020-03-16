using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
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

        public async Task<IEnumerable<Locations>> CreateUserLocationsAsync(string userId, string jsonData)
        {
            var locations = googleLocationParser.Parse(jsonData).ToList();
            await amazonService.UploadCsvData(userId, locations);
           
            return locations;
        }
    }
}
