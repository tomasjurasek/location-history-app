using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Extensions;
using Services.Options;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: FunctionsStartup(typeof(GoogleFileParser.Startup))]
namespace GoogleFileParser
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {

            builder.Services.AddHttpClient("Keboola", s =>
            {
                var apiToken = "1217-47944-seVdmaX63j4XuDku6HcoHRagJinEx3fvcQ5RGdto";
                s.BaseAddress = new Uri("https://connection.eu-central-1.keboola.com/");
                s.DefaultRequestHeaders.Add("X-StorageApi-Token", apiToken);
            });

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


            builder.Services.AddOptions<AzureServiceBusOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("Azure").Bind(settings);
                });



            builder.Services.AddServices();

        }
    }
}
