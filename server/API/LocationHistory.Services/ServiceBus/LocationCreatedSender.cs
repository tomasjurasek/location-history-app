using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Services.Options;
using System.Threading.Tasks;

namespace API.Services.ServiceBus
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
