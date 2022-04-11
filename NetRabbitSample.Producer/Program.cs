using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetRabbitSample.Business.Helpers;
using NetRabbitSample.Business.Options;
using RabbitMQ.Client;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetRabbitSample.Producer
{
    internal class Program
    {
        private static RabbitMQOptions _rabbitmqOptions;
        private static Random _random;

        public static void WriteDebugLog(string text)
        {
            string dateStr = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            System.Console.WriteLine(string.Format("[{0}] {1}", dateStr, text));
        }

        static void Main(string[] args)
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            // For use with separate docker container for RabbitMq:
            // docker run -d --hostname rabbitmanaged --name rabbitmanaged -p 8081:15672 -p 5672:5672 rabbitmq:3-management

            WriteDebugLog("Producer started");
            CancellationTokenSource tokenSource = new CancellationTokenSource();

            Task processingTask = startProcessingInBackground(tokenSource.Token);
            ConsoleKeyInfo pressedKey = System.Console.ReadKey();
            while (pressedKey.Key != ConsoleKey.Oem3)
            {
                pressedKey = System.Console.ReadKey();
            }
            tokenSource.Cancel();
            WriteDebugLog("Cancellation requested by user, please wait...");
            System.Console.ReadKey();
            WriteDebugLog("Processing completed");
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
                int messageId = 1;
                RabbitManager rabbitManager = new RabbitManager(WriteDebugLog, channel);
                rabbitManager.CreateQueue("testQueue");

                // Основной цикл обработки
                while (!cancellationToken.IsCancellationRequested)
                {
                    rabbitManager.SendMessageToQueue($"Message number {messageId}", "testQueue");
                    messageId++;
                    await Task.Delay(_random.Next(1000, 3000));
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
