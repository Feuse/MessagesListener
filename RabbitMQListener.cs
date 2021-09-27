using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ServicesInterfaces;
using ServicesInterfaces.Global;
using ServicesModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagesListener
{
    public class RabbitMQListener : IListener, IDisposable
    {
        private readonly IMessageRecievedEventHandler _handler;
        private readonly IAppSettings _settings;
        private IModel _channel;
        public RabbitMQListener(IMessageRecievedEventHandler handler, IAppSettings settings)
        {
            _handler = handler;
            _settings = settings;
        }
        public RabbitMQListener() { }


        public void StartListening()
        {
                _channel = _handler._channel;

                EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);

                _channel.QueueDeclare(queue: _settings.Queue,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                consumer.Received += _handler.ConsumeMessage;

                Console.WriteLine("recieved message");

                _channel.BasicConsume(queue: _settings.Queue,
                                                     autoAck: false,
                                                     consumer: consumer);
                Console.ReadLine();
            
       
        }
        public IConnection CreateConnection()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _settings.HostName
            };
            return factory.CreateConnection();
        }
        public void Dispose()
        {
            //_connection.Dispose();
            _channel.Dispose();
        }
        ~RabbitMQListener()
        {
            Dispose();
        }
    }
}
