namespace Twitter.Stats.Infrastructure.Settings
{
    public class MongoDbSettings
    {
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }

        public CollectionsSettings Collections { get; set; }

    }

    public class CollectionsSettings
    {
        public string Tweets { get; set; }
        public string HashTags { get; set; }
    }
}
