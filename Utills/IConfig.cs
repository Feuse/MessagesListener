using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagesListener.Utills
{
    public interface IConfig
    {
        AutoLoverDatabaseSettings AutoLoverDatabaseSettings { get; }
        ConnectionStrings ConnectionStrings { get; }
    }
}
