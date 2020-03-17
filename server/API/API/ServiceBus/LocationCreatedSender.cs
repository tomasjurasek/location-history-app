using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace API.ServiceBus
{
    public class LocationCreatedSender : ServiceBusSender
    {
        public LocationCreatedSender(IConfiguration configuration) : base(configuration)
        {
        }

        public override Task SendMessageAsync(LocationsCreatedMessage messageBody)
        {
            return base.SendMessageAsync(messageBody);
        }
    }
}
