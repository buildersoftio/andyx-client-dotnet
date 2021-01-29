using Buildersoft.Andy.X.Client.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Client.Events
{
    public class StateChangedContext
    {
        public ConnectionStates ConnectionState { get; private set; }

        public StateChangedContext(ConnectionStates connectionState)
        {
            ConnectionState = connectionState;
        }
    }
}
