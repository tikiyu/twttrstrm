using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Twitter.Stats.Application.Common.Helpers;
using Twitter.Stats.Application.Common.Interfaces;
using Twitter.Stats.Application.Common.Settings;
using Twitter.Stats.Domain.Entities;
using Twitter.Stats.Domain.Events;

namespace Twitter.Stats.Application.Tweets.EventHandlers
{
    public class TweetCreatedEventHandler : INotificationHandler<TweetCreatedEvent>
    {
        private readonly IThreadSafeMemoryCache<string, FrequencyDictionary> _memoryCache;
        private readonly ILogger<TweetCreatedEventHandler> _logger;
        private readonly HashtagSettings _hashtagSettings;
        private readonly IHashTagRepository _hashTagRepository;
        private readonly ParallelOptions _parallelOptions;

        public TweetCreatedEventHandler(
            IThreadSafeMemoryCache<string, FrequencyDictionary> memoryCache,
            ILogger<TweetCreatedEventHandler> logger,
            IOptions<HashtagSettings> hashtagSettings,
            IHashTagRepository hashTagRepository)
        {
            _memoryCache = memoryCache;
            _logger = logger;
            _hashtagSettings = hashtagSettings.Value;
            _hashTagRepository = hashTagRepository;
            _parallelOptions = new()
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
            };
        }

        public async Task Handle(TweetCreatedEvent notification, CancellationToken cancellationToken)
        {
            //_logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().Name);

            if (notification.Tweet.Hashtags.Any())
            {

                AddToCache(notification.Tweet.Hashtags);

                IEnumerable<HashTag> hashTags = notification.Tweet.Hashtags.Select(item => new HashTag
                {
                    CreatedAt = notification.Tweet.CreatedAt,
                    Tag = item
                }).ToList();

                await _hashTagRepository.InsertHashTagsAsync(hashTags, cancellationToken);
            };
        }

        private void AddToCache(IEnumerable<string> hashTags)
        {
            if (!_memoryCache.TryGetValue(nameof(HashTag), value: out FrequencyDictionary frequencyCounter))
            {
                frequencyCounter = new(TimeSpan.FromMinutes(_hashtagSettings.TrendingTtlInMins));
            }

            if (frequencyCounter != null)
            {
                Parallel.ForEach(hashTags, _parallelOptions, frequencyCounter.Add);

                _memoryCache.Set(nameof(HashTag), frequencyCounter);
            }
        }
    }
}
