using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQScheduler.Interfaces;
using RabbitMQScheduler.Models;
using ServicesInterfaces;
using ServicesModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQListenerService
{
    public class RabbitMQListener : IRabbitMQListener, IDisposable
    {
        private  IConnection _connection;
        private  IModel _channel;
        private readonly IServicesFactory _factory;
        private readonly IScheduler _scheduler;
        private readonly IDataAccess _data;

        public RabbitMQListener(IServicesFactory factory, IScheduler scheduler)
        {
            _factory = factory;
            _scheduler = scheduler;
        }

        public void StartListening()
        {
            _connection = CreateConnection();

            _channel = _connection.CreateModel();

            EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);

            _channel.QueueDeclare(queue: "messages",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            int result = 0;
            consumer.Received += async (model, ea) =>
            {
                Console.WriteLine("waiting for queue");
                var _body = ea.Body.ToArray();
                var msg = Encoding.UTF8.GetString(_body);
                var message = JsonConvert.DeserializeObject<Message>(msg);

                IService service = _factory.GetService(message.Service);

                //////////  WAITING FOR XPING SCRIPT  //////////
                //var credentials = await GetUserNamePassword(message);
                
                ///////////////////
                var response = await service.AppStartUp(new Data() {UserName= "x", Password = "x", Likes = message .Likes,XPing= "f71d4842782bd158dc92f78d3a9836c5" });
                var parsedResponse = JsonConvert.DeserializeObject<dynamic>(response);
                if (parsedResponse.session_id != null)
                {
                    result = await service.Like(new Data() {SessionId = parsedResponse.session_id, UserId = parsedResponse.user_id ,Likes=message.Likes});
                    if (result > 0)
                    {
                        _channel.BasicNack(ea.DeliveryTag, false, true);
                    }
                    else
                    {
                        _channel.BasicAck(ea.DeliveryTag, false);
                    }
                }
                else
                {
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }

            };
            _channel.BasicConsume(queue: "messages",
                                                  autoAck: false,
                                                 consumer: consumer);
            Console.WriteLine("done1");
            Console.ReadLine();
            Console.WriteLine("done2");

        }

        public IConnection CreateConnection()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };
            return factory.CreateConnection();
        }
        ~RabbitMQListener()
        {
            Dispose();
        }

        public void Dispose()
        {
            _connection.Dispose();
            _channel.Dispose();
        }

        public Message DeserializeJsonMesage(byte[] msg)
        {
            var message = Encoding.UTF8.GetString(msg);
            return JsonConvert.DeserializeObject<Message>(message);
        }

        public byte[] SerializeMessage(Message msg)
        {
            var newJson = JsonConvert.SerializeObject(msg);
            return Encoding.UTF8.GetBytes(newJson);
        }

        public async Task<UserServiceCredentials> GetUserNamePassword(Message message)
        {
            return await _data.GetById(message);
        }
    }
}
