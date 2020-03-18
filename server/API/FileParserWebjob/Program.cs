using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services;
using Services.Extensions;
using Services.Options;

namespace FileParserWebjob
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {

                    services.AddHostedService<Worker>();
                    services.AddServices();
                    services.Configure<AmazonOptions>(hostContext.Configuration.GetSection("Amazon"));
                    services.Configure<AzureBlobServiceOptions>(hostContext.Configuration.GetSection("Azure"));
                    services.Configure<AzureServiceBusOptions>(hostContext.Configuration.GetSection("Azure"));
                    services.AddHttpClient("Keboola", s =>
                    {
                        var token = hostContext.Configuration.GetValue<string>("KeboolaToken");
                        var apiToken = token;
                        s.BaseAddress = new Uri("https://connection.eu-central-1.keboola.com/");
                        s.DefaultRequestHeaders.Add("X-StorageApi-Token", apiToken);
                    });

                });
    }
}
