using Buildersoft.Andy.X.Client.Abstraction;
using Buildersoft.Andy.X.Client.Configurations;
using Buildersoft.Andy.X.Client.Services.Events;
using System;

namespace Buildersoft.Andy.X.Client
{
    public class Reader<T> : ReaderBase<T>
    {

        public Reader(AndyXClient andyClient) : base(andyClient, typeof(Reader<T>))
        {

        }

        public Reader(AndyXClient andyClient, Action<ReaderOptions> readerOptions) : base(andyClient, typeof(Reader<T>), readerOptions)
        {

        }
    }
}
