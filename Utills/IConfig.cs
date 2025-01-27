﻿using DataAccess;
using DataAccess.Utills;
using Services.Server.Utills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagesListener.Utills
{
    public interface IConfig
    {
        public string APIUrl { get; set; }
        public List<int> QueuePorts { get; set; }
        public string HostName { get; set; }
        public string Queue { get; set; }
        public string ServiceSessionsCollectionName { get; set; }
        public string UserCredentialsCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string InstanceName { get; set; }
        public string InstanceId { get; set; }
        public string JobType { get; set; }
        public string DataSource { get; set; }
        public string TablePrefix { get; set; }
        public string ProviderConnectionString { get; set; }
        public string Provider { get; set; }
        public string Type { get; set; }
    }
}
