using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQListenerService.Interfaces
{
    public interface IRabbitMQListener
    {
        public void StartListening();
    }
}
