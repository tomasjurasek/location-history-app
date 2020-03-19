using System.Threading;
using System.Threading.Tasks;
using API.Services.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Services.ServiceBus;

namespace LocationHistoryParserFunction
{
    public class LocationHistoryParserFunction
    {
        private readonly LocationMessageProcessor locationMessageProcessor;

        public LocationHistoryParserFunction(LocationMessageProcessor locationMessageProcessor)
        {
            this.locationMessageProcessor = locationMessageProcessor;
        }

        [FunctionName("LocationHistoryParserFunction")]
        public Task Run([ServiceBusTrigger("development", Connection = "ServiceBusConnection")]
            string queueMessage, ILogger logger)
        {
            logger.LogInformation($">>> Processing message: {queueMessage}");

            var message = JsonConvert.DeserializeObject<LocationsCreatedMessage>(queueMessage);
            return locationMessageProcessor.ProcessAsync(message, CancellationToken.None);
        }
    }
}