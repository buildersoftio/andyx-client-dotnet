namespace Andy.X.Client.Configurations
{
    public sealed class XClientCredentials
    {
        // this class will be needed for getting the cluster configuration.
        public string Username { get; set; }
        public string Password { get; set; }

        public XClientCredentials()
        {
            Username = "";
            Password = "";
        }
    }
}
