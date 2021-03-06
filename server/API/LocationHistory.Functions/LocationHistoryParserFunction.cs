using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LocationHistory.Services;
using LocationHistory.Services.ServiceBus;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LocationHistory.Functions
{
    public class LocationHistoryParserFunction
    {
        private readonly LocationMessageProcessor locationMessageProcessor;

        public LocationHistoryParserFunction(LocationMessageProcessor locationMessageProcessor)
        {
            this.locationMessageProcessor = locationMessageProcessor;
        }

        [FunctionName("LocationHistoryParserFunction")]
        public async Task Run([ServiceBusTrigger("locationfilequeue", Connection = "ServiceBusConnection")]
            Message message, ILogger logger)
        {
            var messageBody = Encoding.UTF8.GetString(message.Body);
            logger.LogInformation(">>> Processing message '{MessageBody}' {EnqueuedTimeUtc}.", messageBody, message.SystemProperties.EnqueuedTimeUtc);

            var messageData = JsonConvert.DeserializeObject<LocationsCreatedMessage>(messageBody);
            await locationMessageProcessor.ProcessAsync(messageData, CancellationToken.None);
            logger.LogInformation(">>> Finished processing of message '{MessageBody}'.", messageBody);
        }
    }
}