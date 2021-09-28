using Autofac;
using Autofac.Extras.Quartz;
using BadooAPI.Factories;
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
using System.Collections.Specialized;
using Services.Server.Utills;
using ServicesInterfaces.Global;
using Autofac.Extras.NLog;

namespace MessagesListener.Installer
{
    public class InstallerClass
    {

        public static IContainer Startup()
        {

            var builder = new ContainerBuilder();
            var confBuilder = GetSettingsFromFile();
            builder.RegisterType<LoggerFactory>()
    .As<ILoggerFactory>()
    .SingleInstance();
            builder.RegisterGeneric(typeof(Logger<>))
                .As(typeof(ILogger<>))
                .SingleInstance();

            RedisCacheOptions options = new RedisCacheOptions() { Configuration = confBuilder.Item1.GetConnectionString("Redis") };
            var x = typeof(Utills.AppSettings);
            var settings = confBuilder.Item1.GetSection(typeof(Utills.AppSettings).Name).Get<Utills.AppSettings>();

            builder.Register(c => settings).As<IAppSettings>();
            builder.RegisterModule(new AutoMapperModule());
            // builder.RegisterModule<ConfigurationModule<JsonResolver<Config>>>();
            builder.RegisterType<ServicesFactory>().As<IServicesFactory>();

            builder.Register(ctx => new RedisCache(options)).As<IDistributedCache>();
            builder.RegisterType<DataAccessManager>().As<IDataAccessManager>();
            builder.RegisterType<ServicesDataAccess>().As<IDataAccess>();
            builder.RegisterType<ServicesDataAccessCache>().As<ICacheDataAccess>();

            builder.RegisterType<Application>().As<IApplication>();
            builder.RegisterType<RabbitMQListener>().As<IListener>().InstancePerDependency();

            builder.RegisterType<RabbitMQEventHandler>().As<IMessageRecievedEventHandler>();
            //builder.RegisterType<ServicesFactory>().As<IServicesFactory>();
            builder.RegisterType<JsonRequestBodyFactory>().As<IJsonFactory>();
            var instance = QuartzInstance.Instance;
            builder.RegisterType<Scheduler.Scheduler>().AsImplementedInterfaces();
            builder.RegisterType<Queue>().AsImplementedInterfaces();
            builder.RegisterModule(new QuartzAutofacFactoryModule());

            return builder.Build();
        }

        private static (IConfigurationRoot, NameValueCollection) GetSettingsFromFile()
        {
            var confBuilder = new ConfigurationBuilder()
                       //what will be the current directory on production server?
                       .SetBasePath(Environment.CurrentDirectory)
                       .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();

            var values = confBuilder.GetSection("QuartzSettings").GetChildren();
            NameValueCollection collection = new NameValueCollection();
            foreach (var item in values)
            {
                collection.Add(item.Key, item.Value);
            }

            return (confBuilder, collection);
        }
    }
}
