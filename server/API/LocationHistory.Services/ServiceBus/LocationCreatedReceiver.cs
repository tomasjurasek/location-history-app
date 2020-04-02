using System.Threading;
using System.Threading.Tasks;
using LocationHistory.Database;
using LocationHistory.Services.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LocationHistory.Services.ServiceBus
{
    public class LocationCreatedReceiver : ServiceBusReceiver
    {
        private readonly UserLocationsService userLocationsService;
        private readonly AzureBlobLocationFileService azureBlobService;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILoggerFactory loggerFactory;

        public LocationCreatedReceiver(IOptions<AzureServiceBusOptions> options, ILogger<ServiceBusReceiver> logger,
            UserLocationsService userLocationsService, AzureBlobLocationFileService azureBlobService, IServiceScopeFactory serviceScopeFactory,
            ILoggerFactory loggerFactory) : base(options, logger)
        {
            this.userLocationsService = userLocationsService;
            this.azureBlobService = azureBlobService;
            this.serviceScopeFactory = serviceScopeFactory;
            this.loggerFactory = loggerFactory;
        }

        public override async Task ProcessEventAsync(LocationsCreatedMessage message, CancellationToken cancellationToken)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var locationDbContext = scope.ServiceProvider.GetRequiredService<LocationDbContext>();
                var locationMessageProcessor = new LocationMessageProcessor(loggerFactory.CreateLogger<LocationMessageProcessor>(), userLocationsService, azureBlobService, locationDbContext);
                await locationMessageProcessor.ProcessAsync(message, cancellationToken);
            }
        }
    }
}