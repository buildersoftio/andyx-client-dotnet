using System;

namespace Andy.X.Client.Models
{
    public class MessageId
    {
        public Guid IdentityId { get; private set; }
        public int MessageCountAccepted { get; private set; }
        public DateTimeOffset AcceptedDate { get; private set; }

        public MessageId(Guid identityId, int messageCountAccepted, DateTimeOffset acceptedDate)
        {
            IdentityId = identityId;
            MessageCountAccepted = messageCountAccepted;
            AcceptedDate = acceptedDate;
        }
    }
}
