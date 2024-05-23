using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace ServiceBusWebApi.Services
{
    public class ServiceBusSenderService
    {
        private readonly string _connectionString;
        private readonly string _queueName;

        public ServiceBusSenderService(IConfiguration configuration)
        {
            _connectionString = configuration["ServiceBus:ConnectionString"];
            _queueName = configuration["ServiceBus:QueueName"];
        }

        public async Task SendMessageAsync(string messageBody)
        {
            await using var client = new ServiceBusClient(_connectionString);
            ServiceBusSender sender = client.CreateSender(_queueName);

            ServiceBusMessage message = new ServiceBusMessage(messageBody);
            await sender.SendMessageAsync(message);
        }
    }
}
