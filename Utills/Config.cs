using DataAccess;
using DataAccess.Utills;
using Services.Server.Utills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagesListener.Utills
{
    public class Config: IConfig
    {
        public AppSettings AutoLoverDatabaseSettings { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }
    }
}
