using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Client
{
    public partial class Reader<TEntity> where TEntity : new()
    {
        private class ReaderDisconnectedArgs
        {
            public Guid ReaderId { get; set; }
        }
    }
}
