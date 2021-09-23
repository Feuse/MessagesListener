using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ServicesInterfaces;
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
        private IModel _channel;
        public RabbitMQListener(IMessageRecievedEventHandler handler)
        {
            _handler = handler;
        }
        public RabbitMQListener() { }

        public void StartListening()
        {
           
                _channel = _handler._channel;

                EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);

                _channel.QueueDeclare(queue: "messages",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                consumer.Received += _handler.ConsumeMessage;

                Console.WriteLine("recieved message");

                _channel.BasicConsume(queue: "messages",
                                                     autoAck: false,
                                                     consumer: consumer);
                Console.ReadLine();
            
       
        }
        public IConnection CreateConnection()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
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
