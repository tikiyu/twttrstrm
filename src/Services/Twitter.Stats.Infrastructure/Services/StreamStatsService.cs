using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twitter.Stats.Application.Common.Models;

namespace Twitter.Stats.Infrastructure.Services
{
    public interface IStreamStatsService
    {
        TweetStreamStat GetLiveStat();
        void UpdateStat(TweetStreamStat tweetStreamStat);

    }

    public class StreamStatsService : IStreamStatsService
    {
        private TweetStreamStat _tweetStreamStat = new();
        public TweetStreamStat GetLiveStat() => _tweetStreamStat;
 
        public void UpdateStat(TweetStreamStat tweetStreamStat)
        {
            _tweetStreamStat.TotalTweets = tweetStreamStat.TotalTweets;
            _tweetStreamStat.TweetsPerSecond = tweetStreamStat.TweetsPerSecond;
        }
    }
}
