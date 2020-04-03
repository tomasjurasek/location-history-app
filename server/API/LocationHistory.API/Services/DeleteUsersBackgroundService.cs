using LocationHistory.Database;
using LocationHistory.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LocationHistory.API.Services
{
    public class DeleteUsersBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly UserLocationsService userLocationsService;

        public DeleteUsersBackgroundService(IServiceScopeFactory serviceScopeFactory, UserLocationsService userLocationsService)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.userLocationsService = userLocationsService;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<LocationDbContext>();

                    var users = context.Users.Where(s => s.CreatedDateTime <= DateTime.Now.AddHours(-8));

                    foreach (var user in users)
                    {
                        await userLocationsService.DeleteUserData(user.UserIdentifier);
                    }

                    context.Users.RemoveRange(users);
                    await context.SaveChangesAsync();
                }

                await Task.Delay(TimeSpan.FromMinutes(30));
            }
        }
    }
}
