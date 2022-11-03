namespace Andy.X.Security.Credentials
{
    public sealed class XClientCredentials
    {
        // this class will be needed for getting the cluster configuration.
        public string Username { get; private set; }
        public string Password { get; private set; }

        public XClientCredentials(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
