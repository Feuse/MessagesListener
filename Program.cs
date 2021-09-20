using Autofac;
using Divergic.Configuration.Autofac;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Quartz;
using RabbitMQListenerService.AppWrapper;
using RabbitMQListenerService.Installer;
using RabbitMQListenerService.Utills;
using RabbitMQScheduler;
using System;

namespace RabbitMQListenerService
{
    public class Program
    {
        static void Main(string[] args)
        {
            var container = InstallerClass.Startup();
            using (var scope = container.BeginLifetimeScope())
            {
                var configuration = container.Resolve<Config>();
               
                var app = scope.Resolve<IApplication>();
                var _scheduler = container.Resolve<IScheduler>();
                _scheduler.JobFactory = new ListenerJobFactory(container);
                _scheduler.Start();

                app.Run();

            }
        }
    }
}
