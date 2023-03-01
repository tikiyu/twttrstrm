using Microsoft.AspNetCore.SignalR;
using Twitter.Stats.Application.Common.Settings;
using Twitter.Stats.Application.HashTags.Queries.GetTrendingHashTags;
using Twitter.Stats.Infrastructure.Services;

namespace Twitter.Stats.API.Hubs
{
    public class TweetStatsHub : Hub
    {
        private readonly ILogger<TweetStatsHub> _logger;
        private readonly IStreamStatsService _streamStatsService;
        private readonly System.Timers.Timer _timer;

        public TweetStatsHub(IStreamStatsService streamStatsService, ILogger<TweetStatsHub> logger) 
        {
            _streamStatsService = streamStatsService;
            _timer = new System.Timers.Timer
            {
                Interval = 1000
            };

            _timer.Elapsed += async (sender, e) => await PushTweetStats();
            _timer.Start();
            _logger = logger;
        }

        private async Task PushTweetStats()
        {

            var tweetStat = _streamStatsService.GetLiveStat();

            await Clients.All.SendAsync("ReceiveTweetStats", tweetStat);

            _logger.LogInformation("{class}:{method}", nameof(TweetStatsHub) ,nameof(PushTweetStats));
        }


    }
}
