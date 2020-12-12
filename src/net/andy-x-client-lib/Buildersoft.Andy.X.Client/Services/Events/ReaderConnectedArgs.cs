using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Client
{
    public partial class Reader<TEntity> where TEntity : new()
    {
        private class ReaderConnectedArgs
        {
            public Guid ReaderId { get; set; }
        }
    }
}
