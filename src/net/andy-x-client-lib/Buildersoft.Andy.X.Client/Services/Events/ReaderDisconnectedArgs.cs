using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Client
{
    public partial class Reader<TEntity>
    {
        private class ReaderDisconnectedArgs
        {
            public Guid ReaderId { get; set; }
        }
    }
}
