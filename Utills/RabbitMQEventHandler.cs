using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQScheduler.Models;
using ServicesInterfaces;
using ServicesModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQListenerService.Utills
{
    public class RabbitMQEventHandler : IMessageRecievedEventHandler, IDisposable
    {
        public IModel _channel { get; }
        private IConnection _connection;
        private readonly IServicesFactory _factory;
        private readonly IDataAccessManager _dataManager;

        public RabbitMQEventHandler(IServicesFactory factory, IDataAccessManager data)
        {
            _dataManager = data;
            _factory = factory;
            _connection = CreateConnection();
            _channel = _connection.CreateModel();
        }
        public IConnection CreateConnection()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };
            return factory.CreateConnection();
        }

        public async void ConsumeMessage(object model, BasicDeliverEventArgs ea)
        {
            var _body = ea.Body.ToArray();
            var msg = Encoding.UTF8.GetString(_body);
            var message = JsonConvert.DeserializeObject<Message>(msg);
            Console.WriteLine("New message : "+ message.MessageId);
            IService service = _factory.GetService(message.Service);

            var credentials = await GetUserNamePassword(message);

            var response = await service.AppStartUp(new Data() { UserName = credentials.Username, Password = credentials.Password, Likes = message.Likes });

            try
            {

            if (response.Result == Result.Success)
            {
                int result = 0;
                result = await service.Like(new Data() { SessionId = response.SessionId, UserServiceId = response.UserServiceId, Likes = message.Likes });
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
                await _dataManager.RemoveServiceFromUser(new Data() { Id = message.UserId, Service = message.Service });
                _channel.BasicNack(ea.DeliveryTag, false, true);
            }
            }
            catch (Exception)
            {
               // _channel.BasicNack(ea.DeliveryTag, false, true);
            }
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }
        ~RabbitMQEventHandler()
        {
            Dispose();
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
            return await _dataManager.GetUserServiceByServiceNameAndId(new Data() { Service = message.Service, Id = message.UserId });
        }
    }
}
