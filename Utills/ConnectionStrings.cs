using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQListenerService.Utills
{
    public class ConnectionStrings : IConnectionStrings
    {
        public string Redis { get; set; }
    }
}
