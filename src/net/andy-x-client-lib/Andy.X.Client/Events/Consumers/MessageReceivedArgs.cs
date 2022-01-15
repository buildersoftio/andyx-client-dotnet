using System;

namespace Andy.X.Client.Events.Consumers
{
    public class MessageReceivedArgs<T>
    {
        public string Tenant { get; private set; }
        public string Product { get; private set; }
        public string Component { get; private set; }
        public string Topic { get; private set; }

        public Guid MessageId { get; private set; }

        //
        // Summary:
        //     Gets the message data as a System.Object.
        public object Payload { get; private set; }
        public T GenericPayload { get; private set; }

        public DateTime SentDate { get; private set; }
        public DateTime ReceivedDate { get; private set; }



        public MessageReceivedArgs(string tenant, string product, string component, string topic, Guid messageId, object payload, T genericPayload, DateTime sentDate)
        {
            Tenant = tenant;
            Product = product;
            Component = component;
            Topic = topic;

            MessageId = messageId;
            Payload = payload;
            GenericPayload = genericPayload;

            SentDate = sentDate;
            ReceivedDate = DateTime.UtcNow;
        }
    }
}
