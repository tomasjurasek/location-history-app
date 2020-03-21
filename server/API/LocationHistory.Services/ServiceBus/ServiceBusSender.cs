using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Services.Options;
using System.Text;
using System.Threading.Tasks;

namespace API.Services.ServiceBus
{
    public abstract class ServiceBusSender
    {
        protected readonly IQueueClient _queueClient;

        public ServiceBusSender(IOptions<AzureServiceBusOptions> options)
        {
            var cs = options.Value.ServiceBus;
            var queueName = options.Value.QueueName;
            _queueClient = new QueueClient(cs, queueName);
        }

        public virtual Task SendMessageAsync(LocationsCreatedMessage messageBody)
        {
            var jsonBody = JsonConvert.SerializeObject(messageBody);

            var message = new Message(Encoding.UTF8.GetBytes(jsonBody));
            return _queueClient.SendAsync(message);
        }
    }
}
