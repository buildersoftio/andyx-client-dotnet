﻿using MessagePack;
using System;
using System.Collections.Generic;

namespace Andy.X.Client.Events.Consumers
{
    [MessagePackObject]
    public sealed class MessageInternalReceivedArgs
    {
        [Key(0)]
        public long EntryId { get; set; }

        [Key(1)]
        public string NodeId { get; set; }

        [Key(2)]
        public Dictionary<string, string> Headers { get; set; }

        [Key(3)]
        public byte[] MessageId { get; set; }

        [Key(4)]
        public byte[] Payload { get; set; }

        [Key(5)]
        public DateTimeOffset SentDate { get; set; }
    }
}
