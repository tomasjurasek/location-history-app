using LocationHistory.Services.BlobStorage;
using LocationHistory.Services.ServiceBus;
using Microsoft.Extensions.DependencyInjection;

namespace LocationHistory.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {

            services.AddSingleton<GoogleLocationParser>();
            services.AddTransient<UserLocationsService>();
            services.AddTransient<LocationCreatedSender>();
            services.AddTransient<LocationCreatedReceiver>();
            services.AddSingleton<AzureBlobLocationFileService>();
            services.AddSingleton<AzureBlobLocationDataFileService>();

            return services;
        }
    }
}
