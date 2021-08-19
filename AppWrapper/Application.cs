using Quartz;
using ServicesInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQListenerService.AppWrapper
{
    public class Application : IApplication
    {
        private readonly IRabbitMQListener _queue;

        public Application(IRabbitMQListener queue)
        {
            _queue = queue;

        }

        public void Run()
        {
            _queue.StartListening();
        }
    }
}
