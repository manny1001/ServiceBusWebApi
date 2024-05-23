using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ServiceBusWebApi.Services
{
    public class ServiceBusReceiverService : IHostedService
    {
        private readonly string _connectionString;
        private readonly string _queueName;
        private readonly ILogger<ServiceBusReceiverService> _logger;
        private ServiceBusProcessor _processor;

        public ServiceBusReceiverService(IConfiguration configuration, ILogger<ServiceBusReceiverService> logger)
        {
            _connectionString = configuration["ServiceBus:ConnectionString"];
            _queueName = configuration["ServiceBus:QueueName"];
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var client = new ServiceBusClient(_connectionString);
            _processor = client.CreateProcessor(_queueName, new ServiceBusProcessorOptions());

            _processor.ProcessMessageAsync += MessageHandler;
            _processor.ProcessErrorAsync += ErrorHandler;

            await _processor.StartProcessingAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _processor.StopProcessingAsync();
            await _processor.DisposeAsync();
        }

        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            Console.WriteLine(body);
            _logger.LogInformation($"Received message: {body}");

            // Process the message
            // Simulate processing
            await Task.Delay(1000);

            // Complete the message. Messages is deleted from the queue.
            await args.CompleteMessageAsync(args.Message);
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception, "Message handler encountered an exception");
            return Task.CompletedTask;
        }
    }
}
