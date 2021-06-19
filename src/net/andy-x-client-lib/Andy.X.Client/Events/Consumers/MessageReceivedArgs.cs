namespace Andy.X.Client.Events.Consumers
{
    public class MessageReceivedArgs
    {
        //
        // Summary:
        //     Gets the message data as a System.Object.
        public object Data { get; private set; }

        public MessageReceivedArgs(object data)
        {
            Data = data;
        }
    }
}
