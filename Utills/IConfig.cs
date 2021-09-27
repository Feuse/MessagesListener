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
    public interface IConfig
    {
        AppSettings AutoLoverDatabaseSettings { get; }
        ConnectionStrings ConnectionStrings { get; }
    }
}
