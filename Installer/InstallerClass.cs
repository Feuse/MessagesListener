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
using ServicesInterfaces.DataAccess;
using DataAccess.Cache;
using ServicesInterfaces.DataAccess.Cache;

namespace MessagesListener.Installer
{
    public class InstallerClass
    {

        public static IContainer Startup()
        {
            var builder = new ContainerBuilder();
            var confBuilder = GetSettingsFromFile();

            #region Loggers
            builder.RegisterType<LoggerFactory>()
                   .As<ILoggerFactory>()
                   .SingleInstance();

            builder.RegisterGeneric(typeof(Logger<>))
                .As(typeof(ILogger<>))
                .SingleInstance();
            #endregion

            #region Configuration File
            var settings = confBuilder.Item1.GetSection(typeof(Utills.AppSettings).Name).Get<Utills.AppSettings>();
            builder.Register(c => settings).As<IAppSettings>();
            #endregion

            #region Loggers
            RedisCacheOptions options = new RedisCacheOptions() { Configuration = confBuilder.Item1.GetConnectionString("Redis") };
            builder.Register(ctx => new RedisCache(options)).As<IDistributedCache>();
            #endregion

            #region Repositories
            var cacheOptions = new DistributedCacheEntryOptions()
                   .SetAbsoluteExpiration(TimeSpan.FromSeconds(60));
            builder.Register(c => cacheOptions).As<DistributedCacheEntryOptions>();
            builder.RegisterType<DataAccessManager>().As<IDataAccessManager>();
            builder.RegisterType<ServicesDataAccess>().As<IServiceDataAccess>();
            builder.RegisterType<UserDataAccess>().As<IUserDataAccess>();
            builder.RegisterType<ServicesCacheAccess>().As<IServiceCacheAccess>();
            builder.RegisterType<UserCacheAccess>().As<IUserCacheAccess>();
            #endregion

            #region Services
            builder.RegisterType<ServicesFactory>().As<IServicesFactory>();
            builder.RegisterType<JsonRequestBodyFactory>().As<IJsonFactory>();
            #endregion

            #region Queue Listeners
            builder.RegisterType<RabbitMQListener>().As<IListener>().SingleInstance();
            builder.RegisterType<RabbitMQEventHandler>().As<IMessageRecievedEventHandler>();
            builder.RegisterType<Queue>().AsImplementedInterfaces();
            #endregion

            #region Schedulers
            var instance = QuartzInstance.Instance;
            builder.RegisterType<Scheduler.Scheduler>().AsImplementedInterfaces();
            builder.RegisterModule(new QuartzAutofacFactoryModule());
            #endregion

            #region Utills
            builder.RegisterModule(new AutoMapperModule());
            builder.RegisterType<Application>().As<IApplication>();
            #endregion

            return builder.Build();
        }

        private static (IConfigurationRoot, NameValueCollection) GetSettingsFromFile()
        {
            var confBuilder = new ConfigurationBuilder()
                       //what will be the current directory on production server?
                       .SetBasePath(Environment.CurrentDirectory)
                       .AddJsonFile(@"C:\Users\Feuse135\source\repos\Services.Server\appsettings.json", optional: true, reloadOnChange: true).Build();

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
