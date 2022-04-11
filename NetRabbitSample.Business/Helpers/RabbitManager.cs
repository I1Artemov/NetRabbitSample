using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetRabbitSample.Business.Helpers
{
    public class RabbitManager
    {
        private readonly Action<string> _debugLog;
        private readonly IModel _rabbitChannel;

        public RabbitManager(Action<string> debugLog, IModel rabbitChannel)
        {
            _debugLog = debugLog;
            _rabbitChannel = rabbitChannel;
        }

        public void CreateQueue(string name)
        {
            _rabbitChannel.QueueDeclare(queue: name,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _debugLog($"Queue {name} created");
        }

        public void SendMessageToQueue(string message, string routingKey)
        {
            var messageBody = Encoding.UTF8.GetBytes(message);
            _rabbitChannel.BasicPublish(exchange: "",
                routingKey: routingKey,
                basicProperties: null,
                body: messageBody);

            _debugLog($" [x] Sent {message}");
        }
    }
}
