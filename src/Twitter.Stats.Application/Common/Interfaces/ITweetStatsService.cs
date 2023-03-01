using Twitter.Stats.Application.Common.Models;

namespace Twitter.Stats.Application.Common.Interfaces
{
    public interface ITweetStatsService
    {
        Task SendTweetStatAsync(TweetStreamStat tweetStreamStat);
    }
}
