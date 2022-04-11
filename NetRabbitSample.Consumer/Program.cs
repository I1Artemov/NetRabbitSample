using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetRabbitSample.Business.Helpers;
using NetRabbitSample.Business.Options;
using RabbitMQ.Client;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NetRabbitSample.Consumer
{
    internal class Program
    {
        private static RabbitMQOptions _rabbitmqOptions;
        private static Random _random;

        static void Main(string[] args)
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            Console.WriteLine("Consumer started!");
            CancellationTokenSource tokenSource = new CancellationTokenSource();

            Task processingTask = startProcessingInBackground(tokenSource.Token);
            ConsoleKeyInfo pressedKey = System.Console.ReadKey();
            while (pressedKey.Key != ConsoleKey.Oem3)
            {
                pressedKey = System.Console.ReadKey();
            }
            tokenSource.Cancel();
            Utils.WriteDebugLog("Consumer cancellation requested, please wait...");
            System.Console.ReadKey();
            Utils.WriteDebugLog("Consumer processing completed");
        }

        private static async Task startProcessingInBackground(CancellationToken cancellationToken)
        {
            ConnectionFactory factory = new ConnectionFactory()
            {
                HostName = _rabbitmqOptions.RabbitHostname,
                Port = _rabbitmqOptions.RabbitPort
            };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                RabbitConsumer rabbitManager = new RabbitConsumer(Utils.WriteDebugLog, channel);
                rabbitManager.CreateQueue("testQueue"); // Если продьюсер не успел создать очередь
                rabbitManager.AddMessageProcessingHandler();

                // Основной цикл обработки
                while (!cancellationToken.IsCancellationRequested)
                {
                    rabbitManager.ProcessSingleMessageFromQueue("testQueue");
                    await Task.Delay(_random.Next(1000, 2000));
                }
            }
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            _rabbitmqOptions = configuration.GetSection(nameof(RabbitMQOptions)).Get<RabbitMQOptions>();
            _random = new Random();
        }
    }
}
