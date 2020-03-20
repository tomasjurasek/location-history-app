using System;
using Database;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services;
using Services.Options;
using Services.ServiceBus;

[assembly: FunctionsStartup(typeof(LocationHistoryParserFunction.Startup))]

namespace LocationHistoryParserFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var connectionString = Environment.GetEnvironmentVariable("SqlConnectionString") ?? throw new Exception("Missing configuration of 'SqlConnectionString' parameter.");
            builder.Services.AddDbContext<LocationDbContext>(
                options => options.UseSqlServer(connectionString));

            builder.Services.AddLogging();
            builder.Services.AddTransient<LocationMessageProcessor>();
            builder.Services.AddTransient<UserLocationsService>();
            builder.Services.AddTransient<AzureBlobService>();
            builder.Services.AddTransient<GoogleLocationParser>();
            builder.Services.AddTransient<AmazonService>();

            builder.Services.AddOptions<AmazonOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("Amazon").Bind(settings);
                });

            builder.Services.AddOptions<AzureBlobServiceOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("Azure").Bind(settings);
                });
        }
    }
}