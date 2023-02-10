using MongoDB.Driver;
using Twitter.Stats.Domain.Entities;

namespace Twitter.Stats.Application.Common.Interfaces
{
    public interface IApplicationDb
    {
        IMongoDatabase Database { get; }
        IMongoCollection<Tweet> Tweets { get; }
        IMongoCollection<HashTag> HashTags { get; }
    }
}
