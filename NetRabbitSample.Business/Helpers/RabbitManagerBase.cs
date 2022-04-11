using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Text;

namespace NetRabbitSample.Business.Helpers
{
    public class RabbitManagerBase
    {
        protected readonly Action<string> _debugLog;
        protected readonly IModel _rabbitChannel;

        public RabbitManagerBase(Action<string> debugLog, IModel rabbitChannel)
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
    }
}
