using Quartz;
using ServicesInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessagesListener.AppWrapper
{
    public class Application : IApplication
    {
        private readonly IListener _listener;

        public Application(IListener listener)
        {
            _listener = listener;

        }
        public void Run()
        {
            _listener.StartListening();
        }
    }
}
