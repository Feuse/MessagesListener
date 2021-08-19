using Autofac;
using Quartz;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQListenerService.Utills
{
    public class ListenerJobFactory : IJobFactory
    {
        private readonly IContainer _container;
        public ListenerJobFactory(IContainer container)
        {
            _container = container;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {

            // var service =  _container.ResolveOptional<ConsumerSchechulerJob>();

            return (IJob)_container.Resolve(bundle.JobDetail.JobType);
        }

        public void ReturnJob(IJob job) { }
    }
}
