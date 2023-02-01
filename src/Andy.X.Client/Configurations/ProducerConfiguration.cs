using Andy.X.Client.Abstractions.Serializers;
using MessagePack;
using System.Collections.Generic;

namespace Andy.X.Client.Configurations
{
    public class ProducerConfiguration
    {
        public string Name { get; set; }

        public ProducerSettings Settings { get; set; }
        public ComponentConfiguration Component { get; set; }
        public TopicConfiguration Topic { get; set; }

        // These headers will be sent with each message.
        public Dictionary<string, string> Headers { get; set; }

        public ProducerConfiguration() : this("hello-world-producer", new ProducerSettings())
        {
            // it calls the last constructor
        }

        public ProducerConfiguration(string name) : this(name, new ProducerSettings())
        {
            // it calls the other constructor
        }

        public ProducerConfiguration(string name, ProducerSettings producerSettings)
        {
            Name = name;
            Settings = producerSettings;

            Component = new ComponentConfiguration();
            Topic = new TopicConfiguration();

            Headers = new Dictionary<string, string>();
        }
    }

    public class ProducerSettings
    {
        public IMessageSerializer MessageSerializer { get; private set; } = null;

        public bool EnableRetryProducing { get; set; }
        public bool BreakIfTryToSendMessageInClosedConneciton { get; set; }
        public int BatchSize { get; set; }
        public long LingerMs { get; set; }
        public int RequestTimeoutMs { get; set; }
        public int TimeoutInSyncResponseMs { get; set; }

        public bool RequireCallback { get; set; }

        public CompressionType CompressionType { get; set; }

        public ProducerSettings()
        {
            EnableRetryProducing = false;
            BatchSize = 100;
            LingerMs = 1;
            RequestTimeoutMs = 60000;
            TimeoutInSyncResponseMs = 3000;
            BreakIfTryToSendMessageInClosedConneciton = false;
            RequireCallback = true;

            CompressionType = CompressionType.None;
        }

        public void AddCustomMessageSerializer(IMessageSerializer messageSerializer)
        {
            MessageSerializer = messageSerializer;
        }
    }
}
