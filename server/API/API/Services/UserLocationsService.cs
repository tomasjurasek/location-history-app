using API.Database;
using API.Database.Entities;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.EntityFrameworkCore;
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
        private readonly AmazonService amazonService;

        public UserLocationsService(GoogleLocationParser googleLocationParser, LocationHistoryDbContext dbContext, AmazonService amazonService)
        {
            this.googleLocationParser = googleLocationParser;
            this.dbContext = dbContext;
            this.amazonService = amazonService;
        }


        public async Task CreateUserLocationsAsync(string userId, string jsonData)
        {
            var csv = googleLocationParser.ParseToCsv(userId, jsonData);
            await amazonService.UploadCsvData(userId, csv);
            var locations = googleLocationParser.Parse(jsonData);

            var user = await dbContext.Users.FirstOrDefaultAsync(s => s.Id == userId);
            if(user == null)
            {
                dbContext.Users.Add(new User { Id = userId });
                //await dbContext.SaveChangesAsync();

            }
            var usersLocations = locations.Select(s => new UserLocations
            {
                UserId = userId,
                Latitude = s.Latitude,
                Longitude = s.Longitude,
                DateTimeUtc = s.DateTimeUtc
            });

            dbContext.UsersLocations.AddRange(usersLocations);
            await dbContext.SaveChangesAsync();


        }

        public List<UserLocations> GetUserLocations(string userId)
        {
            return dbContext.UsersLocations.Where(s => s.UserId == userId)
                .ToList();
        }

        public List<UserLocations> GetUserLocations()
        {
            return dbContext.UsersLocations
                .ToList();
        }
    }
}
