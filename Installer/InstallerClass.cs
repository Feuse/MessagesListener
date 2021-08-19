using Autofac;
using Autofac.Extras.Quartz;
using BadooAPI.Factories;
using BadooAPI.Interfaces;
using DataAccess;
using Microsoft.Extensions.Logging;
using RabbitMQListenerService.AppWrapper;
using RabbitMQScheduler;
using RabbitMQScheduler.ServicesImpl;
using ServicesInterfaces;

namespace RabbitMQListenerService.Installer
{
    public class InstallerClass
    {


        public static IContainer Startup()
        {
            var builder = new ContainerBuilder();
            //var contextOptions = new DbContextOptionsBuilder<AppDbContext>();
            //contextOptions.UseSqlServer("server=(localdb)\\MSSQLLocalDB;database=databasename;Trusted_Connection=true");
            //builder.RegisterInstance(contextOptions.Options).As<DbContextOptions<AppDbContext>>();

            builder.RegisterType<DataAccess.DataAccess>().As<IDataAccess>();
            // builder.RegisterType<IdentityDbContext>();
            //builder.RegisterType<AppDbContext>().InstancePerDependency();

            builder.RegisterType<Application>().As<IApplication>();
            builder.RegisterType<RabbitMQListener>().As<IRabbitMQListener>().InstancePerDependency();
            builder.RegisterType<ServicesFactory>().As<IServicesFactory>();

            // builder.RegisterType<Mailer>().As<IMailer>();
            // StmpSettings stmp = new StmpSettings() { Password = "fjodyhkoiuyuuvkh", Port = 587, SenderEmail = "Feuse135@gmail.com", SenderName = "AutoLover", Server = "smtp.gmail.com", Username = "Feuse135@gmail.com" };

            //builder.RegisterType<Mailer>()
            //.As<IMailer>()
            //.WithParameter(
            // new ResolvedParameter(
            //  (pi, ctx) => pi.ParameterType == typeof(StmpSettings) && pi.Name == "stmpSettings",
            //  (pi, ctx) => stmp));


            //builder.RegisterType<LoggerFactory>()
            //            .As<ILoggerFactory>()
            //            .SingleInstance();
            //builder.RegisterGeneric(typeof(Logger<>))
            //                .As(typeof(ILogger<>))
            //                .SingleInstance();

            builder.RegisterType<JsonFactory>().As<IJsonFactory>();
            var instance = QuartzInstance.Instance;
            builder.RegisterType<QueueImpl>().AsImplementedInterfaces();
            builder.RegisterType<Scheduler>().AsImplementedInterfaces();
            // builder.RegisterType<SchedulerImpl>().AsImplementedInterfaces();
            // builder.RegisterModule(new QuartzAutofacJobsModule(typeof(ConsumerSchedulerJob).Assembly)).RegisterAssemblyModules();
            //builder.RegisterModule(new QuartzAutofacJobsModule(typeof(DailyCleanUpJob).Assembly)).RegisterAssemblyModules();
            builder.RegisterModule(new QuartzAutofacFactoryModule());
            // builder.RegisterInstance(QuartzInstance.Instance).AsImplementedInterfaces();
            return builder.Build();

        }
    }
}
