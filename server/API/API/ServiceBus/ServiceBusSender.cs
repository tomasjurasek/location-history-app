using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.ServiceBus
{
    public abstract class ServiceBusSender
    {
        protected readonly IQueueClient _queueClient;

        public ServiceBusSender(IConfiguration configuration)
        {
            var cs = configuration.GetValue<string>("AzureServiceBus");
            _queueClient = new QueueClient(cs, "locationfilequeue");
        }

        public virtual Task SendMessageAsync(LocationsCreatedMessage messageBody)
        {
            var jsonBody = JsonConvert.SerializeObject(messageBody);

            var message = new Message(Encoding.UTF8.GetBytes(jsonBody));
            return _queueClient.SendAsync(message);
        }

    }
}
