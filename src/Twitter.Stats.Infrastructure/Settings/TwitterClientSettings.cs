namespace Twitter.Stats.Infrastructure.Settings
{
    public class TwitterClientSettings
    {
        public string BaseAddress { get; set; }
        public EndpointSettings Endpoints { get; set; }
        public SecretSettings Secrets { get; set; }
        public HeaderSettings Headers { get; set; }
    }

    public class EndpointSettings
    {
        public string VolumeStream { get; set; }
    }

    public class HeaderSettings
    {
        public string Accept { get; set; }
    }

    public class SecretSettings
    {
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string Token { get; set; }
    }
}
