using System;

namespace Andy.X.Client.Abstractions
{
    public abstract partial class ProducerBase<T>
    {
        private class TransmitMessageArgs
        {
            public string Tenant { get; set; }
            public string Product { get; set; }
            public string Component { get; set; }
            public string Topic { get; set; }

            public Guid Id { get; set; }
            public object MessageRaw { get; set; }
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