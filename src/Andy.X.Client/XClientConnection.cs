namespace Andy.X.Client
{
    public sealed class XClientConnection
    {
        public XConnectionState State { get; private set; }

        public string Server { get; private set; }
        public string Version { get; private set; }
        public XClientConnection(XConnectionState state, string server, string version)
        {
            State = state;
            Server = server;
            Version = version;
        }
    }

    public enum XConnectionState
    {
        Open,
        Closed,
        Invalid,
        Unknown
    }
}
