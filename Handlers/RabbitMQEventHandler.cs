
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ServicesInterfaces;
using ServicesInterfaces.Global;
using ServicesInterfaces.Scheduler;
using ServicesModels;
using ServicesModels.BadooAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagesListener.Utills
{
    public class RabbitMQEventHandler : IMessageRecievedEventHandler
    {
        private readonly IListener _listener;
        private readonly ILogger<RabbitMQEventHandler> _logger;
        private readonly IServicesFactory _factory;
        private readonly IDataAccessManager _dataManager;
        private readonly IQueue _queue;
        public RabbitMQEventHandler(IServicesFactory factory, IDataAccessManager data, IAppSettings _config, ILogger<RabbitMQEventHandler> logger, IQueue queue, IListener listener)
        {
            _logger = logger;
            _dataManager = data;
            _factory = factory;
            _queue = queue;
            _listener = listener;
        }

        public void RegisterToQueueEvent()
        {
            // RabbitMQListener listener = new RabbitMQListener();
            _listener.Message += ProccessMessage;
        }

        protected async void ProccessMessage(string msg)
        {
            Message message = default;
            try
            {
                message = JsonConvert.DeserializeObject<Message>(msg);
                Console.WriteLine("New message : " + message.MessageId);
                IService service = _factory.GetService(message.Service);

                var credentials = await GetUserNamePassword(message);

                var response = await service.AppStartUp(new Data() { Username = credentials.Username, Password = credentials.Password, Likes = message.Likes });

                if (response.Result == Result.Success)
                {
                    var result = await service.Like(new Data() { SessionId = response.SessionId, UserServiceId = response.UserServiceId, Likes = message.Likes });
                    if (!result.IsComplete)
                    {
                        message.Likes = result.LikesLeft;
                        _queue.QueueMessage(message);
                    }
                }
                else
                {
                    await _dataManager.RemoveServiceFromUser(new Data() { Id = message.UserId, Service = message.Service });
                    _queue.QueueMessage(message);
                }
            }
            catch (Exception es)
            {
                _logger.LogError(es.Message);
                _logger.LogTrace(es.StackTrace);
                if (message != null)
                {
                    _queue.QueueMessage(message);
                }
            }
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
