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
        private readonly IHashTagService _hashTagService;

        public TweetCreatedEventHandler(
            IHashTagService hashTagService)
        {
            _hashTagService = hashTagService;
        }

        public async Task Handle(TweetCreatedEvent notification, CancellationToken cancellationToken)
        {
            //_logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().Name);

            if (notification.Tweet.Hashtags.Any())
            {


                IEnumerable<HashTag> hashTags = notification.Tweet.Hashtags.Select(item => new HashTag
                {
                    CreatedAt = notification.Tweet.CreatedAt,
                    Tag = item
                }).ToList();

                await _hashTagService.InsertHashTag(hashTags);

                //AddToCache(notification.Tweet.Hashtags);


                //await _hashTagRepository.InsertHashTagsAsync(hashTags, cancellationToken);
            };
        }

        //private void AddToCache(IEnumerable<string> hashTags)
        //{
        //    if (!_memoryCache.TryGetValue(nameof(HashTag), value: out FrequencyDictionary frequencyCounter))
        //    {
        //        frequencyCounter = new(TimeSpan.FromMinutes(_hashtagSettings.TrendingTtlInMins));
        //    }

        //    if (frequencyCounter != null)
        //    {
        //        Parallel.ForEach(hashTags, _parallelOptions, frequencyCounter.Add);

        //        _memoryCache.Set(nameof(HashTag), frequencyCounter);
        //    }
        //}
    }
}
