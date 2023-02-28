using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using MongoDB.Driver;
using System.Threading;
using Tweetinvi.Models.V2;
using Twitter.Stats.Application.Common.Helpers;
using Twitter.Stats.Application.Common.Interfaces;
using Twitter.Stats.Application.Common.Settings;
using Twitter.Stats.Application.Tweets.EventHandlers;
using Twitter.Stats.Domain.Entities;

namespace Twitter.Hashtag.Service.Services
{
    public class HashtagService : HashTagGrpc.HashTagGrpcBase
    {
        private readonly IThreadSafeMemoryCache<string, FrequencyDictionary> _memoryCache;
        private readonly ILogger<TweetCreatedEventHandler> _logger;
        private readonly HashtagSettings _hashtagSettings;
        private readonly IHashTagRepository _hashTagRepository;
        private readonly ParallelOptions _parallelOptions;

        public HashtagService(
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


        public override async Task<Empty> InsertHashTag(HashtagListRequest request, ServerCallContext context)
        {
            _logger.LogInformation("{0} : {1}", nameof(InsertHashTag), request.Hashtags);

            //Parallel.ForEach(request.Hashtags, _parallelOptions, (t) => {


            //    var hashTag = new HashTag
            //    {
            //        Tag = t.Tag,
            //        CreatedAt = t.CreatedAt.ToDateTimeOffset(),

            //    };

            //});
            var tags = request.Hashtags.Select(x => x.Tag );

            AddToCache(tags);

            IEnumerable<HashTag> hashTags = request.Hashtags.Select(item => new HashTag
            {
                CreatedAt = item.CreatedAt.ToDateTimeOffset(),
                Tag = item.Tag
            }).ToList();

            await _hashTagRepository.InsertHashTagsAsync(hashTags, CancellationToken.None);

            return await Task.FromResult(new Empty());
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
