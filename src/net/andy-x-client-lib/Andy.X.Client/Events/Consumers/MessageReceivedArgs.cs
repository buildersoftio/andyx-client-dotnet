using System;

namespace Andy.X.Client.Events.Consumers
{
    public class MessageReceivedArgs<T>
    {
        public Guid MessageId { get; private set; }

        //
        // Summary:
        //     Gets the message data as a System.Object.
        public object RawData { get; private set; }
        public T Data { get; private set; }

        public MessageReceivedArgs(Guid messageId, object rawData, T data)
        {
            MessageId = messageId;
            RawData = rawData;
            Data = data;
        }
    }
}
