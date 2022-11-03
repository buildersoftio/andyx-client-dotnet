using Andy.X.Client.Abstractions.XClients;
using Andy.X.Client.InternalServices;
using Andy.X.Client.Models.Internal;
using Andy.X.Security.Credentials;
using System.Collections.Generic;

namespace Andy.X.Client
{
    public sealed class Cluster
    {
        private readonly IXClient _xClient;
        private readonly XClientCredentials _xClientCredentials;
        private readonly ClusterDetails _clusterDetails;

        public Cluster(IXClient xClient, XClientCredentials xClientCredentials)
        {
            _xClient = xClient;
            _xClientCredentials = xClientCredentials;

            var restService = new XClientHttpService(xClient.GetClientConfiguration());
            _clusterDetails = restService.GetClusterDetails(xClientCredentials.Username, xClientCredentials.Password);

            if (_clusterDetails == null)
            {
                throw new System.Exception("Credentials of Andy X are not correct");
            }
        }

        public string GetClusterName()
        {
            return _clusterDetails.Name;
        }

        public ClusterStatus GetClusterStatus()
        {
            return _clusterDetails.Status;
        }

        public List<Shard> GetShards()
        {
            return _clusterDetails.Shards;
        }
    }
}
