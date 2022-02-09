using System;
using System.Collections.Generic;

namespace Andy.X.Client.Abstractions
{
    public abstract partial class ConsumerBase<T>
    {
        public class MessageInternalReceivedArgs
        {
            public string Tenant { get; set; }
            public string Product { get; set; }
            public string Component { get; set; }
            public string Topic { get; set; }

            public Guid Id { get; set; }
            public object MessageRaw { get; set; }
            public Dictionary<string, object> Headers { get; set; }

            public DateTime SentDate { get; set; }
        }
    }
}