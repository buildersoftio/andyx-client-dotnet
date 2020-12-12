using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Client
{
    public partial class Reader<TEntity> where TEntity : new()
    {
        private class MessageAckDetail
        {
            public string Tenant { get; set; }
            public string Product { get; set; }
            public string Component { get; set; }
            public string Book { get; set; }

            public string Reader { get; set; }

            public string MessageId { get; set; }
        }
    }
}
