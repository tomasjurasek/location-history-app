using System;
using System.Threading;
using System.Threading.Tasks;
using API.Services.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GoogleFileParser
{
    public  class Function1
    {
        private readonly LocationCreatedReceiver locationCreatedReceiver;

        public Function1(LocationCreatedReceiver locationCreatedReceiver)
        {
            this.locationCreatedReceiver = locationCreatedReceiver;
        }
        [FunctionName("Function1")]
        public async Task Run([ServiceBusTrigger("locationfilequeue", Connection = "AzureServiceBus")]string myQueueItem, ILogger log, CancellationToken cancellationToken)
        {
            var eventData = JsonConvert.DeserializeObject<LocationsCreatedMessage>(myQueueItem);
            await locationCreatedReceiver.ProcessEventAsync(eventData, cancellationToken);

            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        }
    }
}
