using Buildersoft.Andy.X.Client.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Client.Events
{
    public class ClientRegisteredContext
    {
        public ClientTypes Type { get; private set; }
        public string Name { get; private set; }
        public ConnectionStates ConnectionState { get; private set; }

        public ClientRegisteredContext(ClientTypes type, string name, ConnectionStates connectionState)
        {
            Type = type;
            Name = name;
            ConnectionState = connectionState;
        }
    }

    public enum ClientTypes
    {
        Writer,
        Reader
    }
}
