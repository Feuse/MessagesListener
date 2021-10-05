using Microsoft.Extensions.Logging;
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
        public readonly IModel _channel;
        public event Action<string> Message;
        private readonly IAppSettings _settings;
        private readonly IConnection _connection;
        private IList<AmqpTcpEndpoint> endpoints;
        private readonly ILogger<RabbitMQListener> _logger;

        public RabbitMQListener(IAppSettings settings, ILogger<RabbitMQListener> logger)
        {
            _logger = logger;
            _settings = settings;
             InitAmqp();
            _connection = CreateConnection();
            _channel = _connection.CreateModel();
        }
        public void InitAmqp()
        {
            try
            {
                endpoints = new List<AmqpTcpEndpoint>();
                foreach (var port in _settings.QueuePorts)
                {
                    endpoints.Add(new AmqpTcpEndpoint(_settings.HostName, port));
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                _logger.LogTrace(e.StackTrace);
            }
        }
        public IConnection CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory() { Uri = new Uri(_settings.AMQP_URL) };
                return factory.CreateConnection();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                _logger.LogTrace(e.StackTrace);
                throw;
            }
        }
        public void StartListening()
        {
            try
            {
                EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);

                _channel.QueueDeclare(queue: _settings.Queue,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
                consumer.Received += (obj, ea) =>
                {
                    var _body = ea.Body.ToArray();
                    var msg = Encoding.UTF8.GetString(_body);

                    Message?.Invoke(msg);

                    _channel.BasicAck(ea.DeliveryTag, true);
                };

                Console.WriteLine("recieved message");

                _channel.BasicConsume(queue: _settings.Queue,
                                                     autoAck: false,
                                                     consumer: consumer);
                Console.ReadLine();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                _logger.LogTrace(e.StackTrace);
                throw;
            }
        }
        public void Dispose()
        {
            _connection.Dispose();
            _channel.Dispose();
        }

        ~RabbitMQListener()
        {
            Dispose();
        }
    }
}
