using Autofac;
using Autofac.Extras.Quartz;
using BadooAPI.Factories;
using BadooAPI.Interfaces;
using DataAccess;
using Divergic.Configuration.Autofac;
using MessagesQueue;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using MessagesListener.AppWrapper;
using MessagesListener.Utills;
using Scheduler;
using ServicesInterfaces;
using System;
using System.Configuration;

namespace MessagesListener.Installer
{
    public class InstallerClass
    {
        public static IConnection CreateConnection()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };
            return factory.CreateConnection();
        }
        public static IContainer Startup()
        {
           var _connection = CreateConnection();

            var _channel = _connection.CreateModel();

            var builder = new ContainerBuilder();

            var confBuilder = new ConfigurationBuilder()
                //what will be the current directory on production server?
           .SetBasePath(Environment.CurrentDirectory)
           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();

            RedisCacheOptions options = new RedisCacheOptions() { Configuration = confBuilder.GetConnectionString("Redis") };

            builder.RegisterModule<ConfigurationModule<JsonResolver<Config>>>();
            
            builder.Register(ctx => new RedisCache(options)).As<IDistributedCache>();
            builder.RegisterType<DataAccessManager>().As<IDataAccessManager>();
            builder.RegisterType<ServicesDataAccess>().As<IDataAccess>();
            builder.RegisterType<ServicesDataAccessCache>().As<ICacheDataAccess>();
           
            builder.RegisterType<Application>().As<IApplication>();
            builder.RegisterType<RabbitMQListener>().As<IListener>().InstancePerDependency();
            
            builder.RegisterType<RabbitMQEventHandler>().As<IMessageRecievedEventHandler>();
            builder.RegisterType<ServicesFactory>().As<IServicesFactory>();
            builder.RegisterType<JsonRequestBodyFactory>().As<IJsonFactory>();
            var instance = QuartzInstance.Instance;
            builder.RegisterType<Scheduler.Scheduler>().AsImplementedInterfaces();
            builder.RegisterType<Queue>().AsImplementedInterfaces();
            builder.RegisterModule(new QuartzAutofacFactoryModule());

            return builder.Build();
        }
    }
}
