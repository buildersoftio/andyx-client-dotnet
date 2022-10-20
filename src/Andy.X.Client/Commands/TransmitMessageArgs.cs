using MessagePack;
using System.Collections.Generic;
using System;

namespace Andy.X.Client.Commands
{
    [MessagePackObject]
    public sealed class TransmitMessageArgs
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
        public byte[] Id { get; set; }

        [Key(6)]
        public byte[] Payload { get; set; }

        [Key(7)]
        public DateTimeOffset SentDate { get; set; }

        [Key(8)]
        public Guid IdentityId { get; set; }

        [Key(9)]
        public string NodeId { get; set; }

        [Key(10)]
        public bool RequiresCallback { get; set; }
    }
}
