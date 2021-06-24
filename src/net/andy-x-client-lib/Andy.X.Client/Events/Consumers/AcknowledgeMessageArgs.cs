using System;

namespace Andy.X.Client.Abstractions
{
    public abstract partial class ConsumerBase<T>
    {
        private class AcknowledgeMessageArgs
        {
            public string Tenant { get; set; }
            public string Product { get; set; }
            public string Component { get; set; }
            public string Topic { get; set; }

            public string Consumer { get; set; }
            public bool IsAcknowledged { get; set; }
            public Guid MessageId { get; set; }
        }
    }
}
