using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Client.Abstraction
{
    public abstract partial class ReaderBase<T>
    {
        private class ReaderConnectedArgs
        {
            public Guid ReaderId { get; set; }
        }
    }
}
