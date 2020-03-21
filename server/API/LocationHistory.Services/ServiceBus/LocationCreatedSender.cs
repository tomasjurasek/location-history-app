using System.Threading.Tasks;
using LocationHistory.Services.Options;
using Microsoft.Extensions.Options;

namespace LocationHistory.Services.ServiceBus
{
    public class LocationCreatedSender : ServiceBusSender
    {
        public LocationCreatedSender(IOptions<AzureServiceBusOptions> options) : base(options)
        {
        }

        public override Task SendMessageAsync(LocationsCreatedMessage messageBody)
        {
            return base.SendMessageAsync(messageBody);
        }
    }
}
