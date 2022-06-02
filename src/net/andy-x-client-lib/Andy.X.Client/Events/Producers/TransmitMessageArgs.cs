using MessagePack;
using System;
using System.Collections.Generic;

namespace Andy.X.Client.Abstractions
{
    public abstract partial class ProducerBase<T>
    {
        [MessagePackObject]
        public class TransmitMessageArgs
        {
            [Key(0)]
            public string Tenant { get; set; }
            [Key(1)]
            public string Product { get; set; }
            [Key(2)]
            public string Component { get; set; }
            [Key(3)]
            public string Topic { get; set; }

            [Key(4)]
            public Dictionary<string, string> Headers { get; set; }

            [Key(5)]
            public string Id { get; set; }

            [Key(6)]
            public byte[] Payload { get; set; }

            [Key(7)]
            public DateTimeOffset SentDate { get; set; }
        }

        private class RetryTransmitMessage
        {
            public TransmitMessageArgs TransmitMessageArgs { get; set; }
            public int RetryCounter { get; set; }
            public RetryTransmitMessage()
            {
                RetryCounter = 0;
            }
        }
    }
}