using API.Services;
using API.Services.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Extensions
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
            services.AddSingleton<AzureBlobService>();

            return services;
        }
    }
}
