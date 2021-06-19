using System;

namespace Andy.X.Client.Events.Consumers
{
    public class MessageReceivedArgs
    {
        public Guid MessageId { get; private set; }

        //
        // Summary:
        //     Gets the message data as a System.Object.
        public object Data { get; private set; }

        public MessageReceivedArgs(Guid messageId, object data)
        {
            MessageId = messageId;
            Data = data;
        }
    }
}
