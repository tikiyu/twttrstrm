using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Twitter.Hashtag.Service.Controllers;
using Twitter.Stats.API.Hubs;
using Twitter.Stats.Application.Common.Settings;
using Twitter.Stats.Application.HashTags.Queries.GetTrendingHashTags;

namespace Twitter.Hashtag.Service.Hubs
{
    public class TrendingHub : Hub
    {
        private readonly ILogger<HashTagsController> _logger;
        private readonly IMediator _mediator;
        private readonly System.Timers.Timer _timer;
        private readonly HashtagSettings _hashtagSettings;

        public TrendingHub(IMediator mediator, ILogger<HashTagsController> logger, IOptions<HashtagSettings> hashtagSettings)
        {
            _mediator = mediator;
            _logger = logger;
            _hashtagSettings = hashtagSettings.Value;
            _timer = new System.Timers.Timer
            {
                Interval = _hashtagSettings.TrendingPushIntervalInMs
            };

            _timer.Elapsed += async (sender, e) => await PushTrendingHashtags();
            _timer.Start();
        }

        private async Task PushTrendingHashtags()
        {
            var trendingHashtags = await _mediator.Send(new GetTrendingHashTagsQuery { TopCount = _hashtagSettings.TopTrendingHashtagCount });

            await Clients.All.SendAsync("ReceiveTrendingHashtags", trendingHashtags);

            _logger.LogInformation("{class}:{method}", nameof(TrendingHub), nameof(PushTrendingHashtags));
        }
    }
}
