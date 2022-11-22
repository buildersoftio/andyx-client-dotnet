using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Andy.X.Client.Models.Internal
{
    internal class ClusterDetails
    {
        public string Name { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DistributionTypes ShardDistributionType { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ClusterStatus Status { get; set; }

        public List<Shard> Shards { get; set; }
    }

    public enum ClusterStatus
    {
        Online,
        PartiallyOnline,
        Starting,
        Offline,
        Restarting,
        Recovering,
        Disconnecting,
    }

    public enum DistributionTypes
    {
        // Sync distribution will not be supported for andyx v3.0.0
        Sync,
        Async,
    }

    public class Shard
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DistributionTypes ReplicaDistributionType { get; set; }
        public List<Replica> Replicas { get; set; }
    }

    public class Replica
    {
        public string NodeId { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ReplicaTypes Type { get; set; }
    }

    public enum ReplicaTypes
    {
        Main,
        Worker,
        BackupReplica
    }
}
