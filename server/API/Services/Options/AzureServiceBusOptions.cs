using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Options
{
    public class AzureServiceBusOptions
    {
        public string ServiceBus { get; set; }
        public string QueueName { get; set; }
    }
}
