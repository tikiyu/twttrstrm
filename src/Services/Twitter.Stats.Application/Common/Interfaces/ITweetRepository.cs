using Twitter.Stats.Domain.Entities;

namespace Twitter.Stats.Application.Common.Interfaces
{
    public interface ITweetRepository
    {
        Task<IList<Tweet>> GetTweetsAsync(CancellationToken cancellationToken);
        Task<Tweet> GetTweetsByIdAsync(string Id, CancellationToken cancellationToken);
        Task InsertTweetAsync(Tweet tweet, CancellationToken cancellationToken);
        Task InsertTweetsAsync(IEnumerable<Tweet> hashTags, CancellationToken cancellationToken);
        Task<long> GetTweetsCountAsync(CancellationToken cancellationToken);
    }
}
