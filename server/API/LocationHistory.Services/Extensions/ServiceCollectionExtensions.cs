using LocationHistory.Services.ServiceBus;
using Microsoft.Extensions.DependencyInjection;

namespace LocationHistory.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {

            services.AddSingleton<GoogleLocationParser>();
            services.AddSingleton<UserLocationsService>();
            services.AddSingleton<AmazonService>();
            services.AddSingleton<LocationCreatedSender>();
            services.AddSingleton<LocationCreatedReceiver>();
            services.AddSingleton<AzureBlobLocationFileService>();
            services.AddSingleton<AzureBlobLocationDataFileService>();

            return services;
        }
    }
}
