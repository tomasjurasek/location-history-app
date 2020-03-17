using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace API.Services.ServiceBus
{
    public abstract class ServiceBusReceiver
    {
        protected readonly IQueueClient _queueClient;
        private readonly ILogger<ServiceBusReceiver> logger;

        public ServiceBusReceiver(IConfiguration configuration, ILogger<ServiceBusReceiver> logger)
        {
            var cs = configuration.GetValue<string>("AzureServiceBus");
            _queueClient = new QueueClient(cs, "locationfilequeue");
            this.logger = logger;
        }

        public virtual void RegisterMessageHandler()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 10,
                AutoComplete = false
            };
            _queueClient.RegisterMessageHandler(ProcessMessageAsync, messageHandlerOptions);
        }

        protected virtual async Task ProcessMessageAsync(Message message, CancellationToken cancellationToken)
        {
            try
            {
                var messageBody = Encoding.UTF8.GetString(message.Body);
                var eventData = JsonConvert.DeserializeObject<LocationsCreatedMessage>(messageBody);

                await ProcessEventAsync(eventData, cancellationToken);
                await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
            }
            catch (Exception ex)
            {
                var messageBody = Encoding.UTF8.GetString(message.Body);
                logger.LogError(ex, $"Could not process message. Message body: {messageBody}");
                throw;
            }
        }

        protected abstract Task ProcessEventAsync(LocationsCreatedMessage message, CancellationToken cancellationToken);

        protected virtual Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            logger.LogError(exceptionReceivedEventArgs.Exception, "Queue receiver error");
            return Task.CompletedTask;
        }
    }
}
