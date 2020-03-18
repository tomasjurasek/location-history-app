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
            services.AddTransient<UserLocationsService>();
            services.AddSingleton<AmazonService>();
            services.AddTransient<LocationCreatedSender>();
            services.AddTransient<LocationCreatedReceiver>();
            services.AddSingleton<AzureBlobService>();

            return services;
        }
    }
}
