using System;
using System.Collections.Generic;

namespace Andy.X.Client.Events.Consumers
{
    public class MessageReceivedArgs<T>
    {
        public string Tenant { get; private set; }
        public string Product { get; private set; }
        public string Component { get; private set; }
        public string Topic { get; private set; }

        public long LedgerId { get; set; }
        public long EntryId { get; set; }

        public string MessageId { get; private set; }

        public Dictionary<string, string> Headers{ get; set; }

        // Summary:
        //     Gets the message data as a byte[].
        public byte[] Payload { get; private set; }
        public T GenericPayload { get; private set; }

        public DateTimeOffset SentDate { get; private set; }
        public DateTimeOffset ReceivedDate { get; private set; }



        public MessageReceivedArgs(string tenant, 
            string product, 
            string component, 
            string topic, 
            long ledgerId,
            long entryId,
            string messageId, 
            Dictionary<string, string> headers, 
            byte[] payload, 
            T genericPayload,
            DateTimeOffset sentDate)
        {
            Tenant = tenant;
            Product = product;
            Component = component;
            Topic = topic;

            LedgerId = ledgerId;
            EntryId = entryId;

            MessageId = messageId;
            Headers = headers;
            Payload = payload;
            GenericPayload = genericPayload;

            SentDate = sentDate;
            ReceivedDate = DateTimeOffset.UtcNow;
        }
    }
}
