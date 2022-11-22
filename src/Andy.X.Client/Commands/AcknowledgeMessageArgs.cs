using MessagePack;

namespace Andy.X.Client.Commands
{
    [MessagePackObject]
    public sealed class AcknowledgeMessageArgs
    {
        [Key(0)]
        public long EntryId { get; set; }

        [Key(1)]
        public string NodeId { get; set; }

        [Key(2)]
        public int Acknowledgement { get; set; }
    }

    public enum MessageAcknowledgement
    {
        Acknowledged,
        Unacknowledged,
        Skipped
    }
}
