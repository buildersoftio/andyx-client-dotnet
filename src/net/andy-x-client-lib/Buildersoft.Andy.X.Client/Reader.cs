using Buildersoft.Andy.X.Client.Abstraction;
using Buildersoft.Andy.X.Client.Services.Events;

namespace Buildersoft.Andy.X.Client
{
    public class Reader<T> : ReaderBase<T>
    {

        public Reader(AndyXClient andyClient) : base(andyClient, typeof(Reader<T>))
        {

        }
    }
}
