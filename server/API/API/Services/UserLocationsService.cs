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

        public UserLocationsService(GoogleLocationParser googleLocationParser, IOptions<CosmosDbOptions> cosmosDbOptions)
        {
            this.googleLocationParser = googleLocationParser;
            var storageAccount = CloudStorageAccount.Parse(cosmosDbOptions.Value.ConnectionString);
            var cloudTableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            table = cloudTableClient.GetTableReference("UserLocations");
            table.CreateIfNotExists();
        }

        public async Task<string> CreateUser(string name)
        {
            var userId = Guid.NewGuid().ToString();
            var userLocations = new UserLocations
            {
                PartitionKey = "UserLocations",
                RowKey = userId,
                Name = name
            };

            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(userLocations);
            await table.ExecuteAsync(insertOrMergeOperation);
           
            return userId;

        }

        public async Task CreateLocations(string userId, string jsonData)
        {
            var locations = googleLocationParser.Parse(jsonData);

            var userLocations = new UserLocations
            {
                PartitionKey = "UserLocations",
                RowKey = userId,
                JsonLocations = JsonConvert.SerializeObject(locations)
            };

            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(userLocations);
            await table.ExecuteAsync(insertOrMergeOperation);
        }

        public async Task<List<Locations>> GetUserLocations(string userId)
        {
            var response = new List<Locations>();
            var retrieveOperation = TableOperation.Retrieve("UserLocations", userId);
            var userLocations = table.CreateQuery<UserLocations>()
                .Where(s => s.PartitionKey == "UserLocations" && s.RowKey == userId)
                .FirstOrDefault();

            response = JsonConvert.DeserializeObject<List<Locations>>(userLocations?.JsonLocations);

            return response;
        }
    }
}
