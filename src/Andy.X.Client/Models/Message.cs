using System;
using System.Collections.Generic;

namespace Andy.X.Client.Models
{
    public sealed class Message<V>
    {
        public long EntryId { get; private set; }
        public string NodeId { get; private set; }

        public IDictionary<string, string> Headers { get; private set; }

        public V Payload { get; private set; }

        public DateTimeOffset SentDate { get; private set; }
        public DateTimeOffset ReceivedDate { get; private set; }

        public Message(long entryId, string nodeId, IDictionary<string, string> headers, V payload, DateTimeOffset sentDate, DateTimeOffset receivedDate)
        {
            EntryId = entryId;
            NodeId = nodeId;
            Headers = headers;
            Payload = payload;
            SentDate = sentDate;
            ReceivedDate = receivedDate;
        }
    }
}
