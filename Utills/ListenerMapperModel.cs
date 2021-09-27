using AutoMapper;
using ServicesModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagesListener.Utills
{
    public class ListenerMapperModel : Profile
    {
        public ListenerMapperModel()
        {
            CreateMap<UserCredentials, Data>();
        }
    }
}
