using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace NetRabbitSample.Business.Helpers
{
    public class RabbitConsumer : RabbitManagerBase
    {
        private readonly EventingBasicConsumer _consumer;

        public RabbitConsumer(Action<string> debugLog, IModel rabbitChannel)
            : base(debugLog, rabbitChannel)
        {
            _consumer = new EventingBasicConsumer(_rabbitChannel);
        }

        public void AddMessageProcessingHandler()
        {
            _consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _debugLog($"[Consumer] Received {message}");
            };
        }

        public void ProcessSingleMessageFromQueue(string routingKey)
        {

            _rabbitChannel.BasicConsume(queue: routingKey,
                autoAck: true,
                consumer: _consumer);
        }
    }
}
