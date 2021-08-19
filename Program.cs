using Autofac;
using Quartz;
using RabbitMQListenerService.AppWrapper;
using RabbitMQListenerService.Installer;
using RabbitMQListenerService.Utills;
using RabbitMQScheduler;
using System;

namespace RabbitMQListenerService
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = InstallerClass.Startup();
            using (var scope = container.BeginLifetimeScope())
            {
                var app = scope.Resolve<IApplication>();
                var _scheduler = container.Resolve<IScheduler>();
                _scheduler.JobFactory = new ListenerJobFactory(container);
                _scheduler.Start();

                app.Run();

            }
        }
    }
}
