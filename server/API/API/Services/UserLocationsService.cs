using API.Database;
using API.Database.Entities;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public class UserLocationsService
    {
        private readonly CloudTable table;
        private readonly GoogleLocationParser googleLocationParser;
        private const string TABLE_NAME = "UserLocations";

        public UserLocationsService(GoogleLocationParser googleLocationParser, IOptions<CosmosDbOptions> cosmosDbOptions)
        {
            this.googleLocationParser = googleLocationParser;
            var storageAccount = CloudStorageAccount.Parse(cosmosDbOptions.Value.ConnectionString);
            var cloudTableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            table = cloudTableClient.GetTableReference(TABLE_NAME);
            table.CreateIfNotExists();
        }


        public async Task CreateUserLocationsAsync(string userId, string jsonData)
        {
            var locations = googleLocationParser.Parse(jsonData);

            var userLocations = new UserLocations
            {
                PartitionKey = TABLE_NAME,
                RowKey = userId,
                JsonLocations = JsonConvert.SerializeObject(locations)
            };

            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(userLocations);
            await table.ExecuteAsync(insertOrMergeOperation);
        }

        public List<Locations> GetUserLocations(string userId)
        {
            var userLocations = table.CreateQuery<UserLocations>()
                .Where(s => s.PartitionKey == TABLE_NAME && s.RowKey == userId)
                .FirstOrDefault();

            var response = JsonConvert.DeserializeObject<List<Locations>>(userLocations?.JsonLocations);
            return response;
        }
    }
}
