using RabbitMQ.Client;
using System;
using System.Text;

namespace NetRabbitSample.Business.Helpers
{
    public class RabbitProducer : RabbitManagerBase
    {
        public RabbitProducer(Action<string> debugLog, IModel rabbitChannel)
            : base (debugLog, rabbitChannel)
        { }
        public void SendMessageToQueue(string message, string routingKey)
        {
            var messageBody = Encoding.UTF8.GetBytes(message);
            _rabbitChannel.BasicPublish(exchange: "",
                routingKey: routingKey,
                basicProperties: null,
                body: messageBody);

            _debugLog($"[Producer] Sent {message}");
        }
    }
}
