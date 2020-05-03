using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarSimulation.Events
{
    public class Event<T> : Event
    {
        public Action<T> Callback;
    }

    public class Event
    {

    }
}
