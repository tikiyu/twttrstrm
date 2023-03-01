using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Twitter.Stats.Application.Common.Interfaces;
using Twitter.Stats.Application.Common.Settings;
using Twitter.Stats.Domain.Entities;

namespace Twitter.Stats.API.Services
{

    public sealed class DayTrendingHashTagService : IDayTrendingHashTagService
    {
        private readonly IHashTagRepository _repository;
        private readonly IMediator _mediator;
        private readonly ILogger<DayTrendingHashTagService> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly HashtagSettings _hashtagSettings;

        public DayTrendingHashTagService(
            IHashTagRepository repository,
            IMediator mediator,
            ILogger<DayTrendingHashTagService> logger,
            IMemoryCache memoryCache,
            IOptions<HashtagSettings> hashtagSettings) =>
            (_repository, _mediator, _logger, _memoryCache, _hashtagSettings) =
            (repository, mediator, logger, memoryCache, hashtagSettings.Value);

        public async Task DoWorkAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(DayTrendingHashTagService)} is running.");

            var hashtags = await _repository.GetHashTagsAsync(cancellationToken);

            //Since limitation on MongoDB In memory doesn't allow me to execute aggregate pipeline, getting the trending hashtags after the repo result
            var trendingdHashtags = hashtags
                                    .GroupBy(x => x.Tag)
                                    .OrderByDescending(x => x.Count())
                                    .Take(_hashtagSettings.DayTrendingCount)
                                    .ToDictionary(g => g.Key, g => g.Count());


            _memoryCache.Set($"Day{nameof(HashTag)}", trendingdHashtags);
        }
    }
}
