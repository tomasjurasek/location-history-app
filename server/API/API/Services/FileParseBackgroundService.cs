using API.ServiceBus;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace API.Services
{
    public abstract class ServiceBusReceiver
    {
        protected readonly IQueueClient _queueClient;
        private readonly ILogger<ServiceBusReceiver> logger;

        public ServiceBusReceiver(IConfiguration configuration, ILogger<ServiceBusReceiver> logger)
        {
            var cs = configuration.GetValue<string>("AzureServiceBus");
            _queueClient = new QueueClient(cs, "locationfilequeue");
            this.logger = logger;
        }

        public virtual void RegisterMessageHandler()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 10,
                AutoComplete = false
            };
            _queueClient.RegisterMessageHandler(ProcessMessageAsync, messageHandlerOptions);
        }

        protected virtual async Task ProcessMessageAsync(Message message, CancellationToken cancellationToken)
        {
            try
            {
                var messageBody = Encoding.UTF8.GetString(message.Body);
                var eventData = JsonConvert.DeserializeObject<LocationsCreatedMessage>(messageBody);

                await ProcessEventAsync(eventData, cancellationToken);
                await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
            }
            catch (Exception ex)
            {
                var messageBody = Encoding.UTF8.GetString(message.Body);
                logger.LogError(ex, $"Could not process message. Message body: {messageBody}");
                throw;
            }
        }

        protected abstract Task ProcessEventAsync(LocationsCreatedMessage message, CancellationToken cancellationToken);

        protected virtual Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            logger.LogError(exceptionReceivedEventArgs.Exception, "Queue receiver error");
            return Task.CompletedTask;
        }
    }

    public class LocationCreatedReceiver : ServiceBusReceiver
    {
        private readonly UserLocationsService userLocationsService;
        private readonly AzureBlobService azureBlobService;

        public LocationCreatedReceiver(IConfiguration configuration, ILogger<ServiceBusReceiver> logger,
            UserLocationsService userLocationsService, AzureBlobService azureBlobService) : base(configuration, logger)
        {
            this.userLocationsService = userLocationsService;
            this.azureBlobService = azureBlobService;
        }

        protected override async Task ProcessEventAsync(LocationsCreatedMessage message, CancellationToken cancellationToken)
        {
            var userId = message.UserId;
            using (var stream = await azureBlobService.DownloadFile(userId))
            {
                if (stream != null)
                {
                    stream.Position = 0;
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), userId);
                    Directory.CreateDirectory(Path.Combine(folderPath));
                    var uploadedFilePath = Path.Combine(folderPath, Path.GetRandomFileName());
                    await using (Stream fileStream = new FileStream(uploadedFilePath, FileMode.Create))
                    {
                        await stream.CopyToAsync(fileStream);
                    }

                    var extractedDirectoryPath = Directory.CreateDirectory(Path.Combine(folderPath, "data"));
                    ZipFile.ExtractToDirectory(uploadedFilePath, extractedDirectoryPath.FullName);

                    var jsonData = GetJsonFilePath(extractedDirectoryPath.FullName);
                    await userLocationsService.CreateUserLocationsAsync(userId, jsonData);

                    Directory.Delete(folderPath, true);
                    await azureBlobService.DeleteFile(userId);
                }
            }
        }

        private string GetJsonFilePath(string directoryPath)
        {
            var jsonPathEn = Path.Combine(directoryPath, "Takeout", "Location History", "Location History.json");
            var jsonPathCz = Path.Combine(directoryPath, "Takeout", "Historie polohy", "Historie polohy.json");

            if (System.IO.File.Exists(jsonPathEn))
            {
                return jsonPathEn;
            }

            if (System.IO.File.Exists(jsonPathCz))
            {
                return jsonPathCz;
            }

            if (!TryGetSingleJsonFile(directoryPath, out var finalJsonPath))
            {
                throw new Exception($"JSON file with location history not found in '{directoryPath}'.");
            }

            return finalJsonPath;
        }

        private bool TryGetSingleJsonFile(string directoryPath, out string jsonFilePath)
        {
            var dir = new DirectoryInfo(directoryPath);
            var files = dir.EnumerateFiles("*.json", SearchOption.TopDirectoryOnly).ToList();
            if (files.Count == 1)
            {
                jsonFilePath = files.Single().FullName;
                return true;
            }

            if (files.Count > 1)
            {
                jsonFilePath = default;
                return false;
            }

            foreach (var subdir in dir.EnumerateDirectories())
            {
                if (TryGetSingleJsonFile(subdir.FullName, out jsonFilePath))
                {
                    return true;
                }
            }

            jsonFilePath = default;
            return false;
        }
    }
    public class FileParseBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        public FileParseBackgroundService(IServiceScopeFactory serviceScopeFactory)
        {

            this.serviceScopeFactory = serviceScopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<LocationCreatedReceiver>();
                service.RegisterMessageHandler();
            }
        }
    }
}
