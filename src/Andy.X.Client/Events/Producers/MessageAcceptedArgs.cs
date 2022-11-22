using MessagePack;
using System;

namespace Andy.X.Client.Events.Producers
{
    [MessagePackObject]
    public sealed class MessageAcceptedArgs
    {
        [Key(0)]
        public Guid IdentityId { get; set; }

        [Key(1)]
        public int MessageCount { get; set; }

        [Key(2)]
        public DateTimeOffset AcceptedDate { get; set; }
    }
}
