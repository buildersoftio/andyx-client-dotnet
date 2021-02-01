using Buildersoft.Andy.X.Client.Abstraction;

namespace Buildersoft.Andy.X.Client
{
    public partial class Reader<T> : ReaderBase<T>
    {
        public Reader(AndyXClient andyClient) : base(andyClient, typeof(Reader<T>))
        {

        }
    }
}
