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
        private readonly GoogleLocationParser googleLocationParser;
        private readonly LocationHistoryDbContext dbContext;

        public UserLocationsService(GoogleLocationParser googleLocationParser, LocationHistoryDbContext dbContext)
        {
            this.googleLocationParser = googleLocationParser;
            this.dbContext = dbContext;
        }


        public async Task CreateUserLocationsAsync(string userId, string jsonData)
        {
            var locations = googleLocationParser.Parse(jsonData);

            var usersLocations = locations.Select(s => new UserLocations
            {
                UserIdentifier = userId,
                Latitude = s.Latitude,
                Longitude = s.Longitude,
                DateTimeUtc = s.DateTimeUtc
            });

            dbContext.UsersLocations.AddRange(usersLocations);
            await dbContext.SaveChangesAsync();


        }

        public List<UserLocations> GetUserLocations(string userId)
        {
            return dbContext.UsersLocations.Where(s => s.UserIdentifier == userId)
                .ToList();
        }

        public List<UserLocations> GetUserLocations()
        {
            return dbContext.UsersLocations
                .ToList();
        }
    }
}
