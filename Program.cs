using Autofac;
using Divergic.Configuration.Autofac;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Quartz;
using MessagesListener.AppWrapper;
using MessagesListener.Installer;
using MessagesListener.Utills;
using Scheduler;
using System;
using Services.Server.Utills;

namespace MessagesListener
{
    public class Program
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
