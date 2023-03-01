using MediatR;
using Twitter.Stats.Application.Common.Helpers;
using Twitter.Stats.Application.Common.Interfaces;
using Twitter.Stats.Domain.Entities;

namespace Twitter.Stats.Application.HashTags.Queries.GetTrendingHashTags
{
    public class GetTrendingHashTagsQueryHandler : IRequestHandler<GetTrendingHashTagsQuery, Dictionary<string, int>>
    {
        private readonly IThreadSafeMemoryCache<string, FrequencyDictionary> _memoryCache;

        public GetTrendingHashTagsQueryHandler(IThreadSafeMemoryCache<string, FrequencyDictionary> memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task<Dictionary<string, int>> Handle(GetTrendingHashTagsQuery request, CancellationToken cancellationToken)
        {
            _memoryCache.TryGetValue(nameof(HashTag), out FrequencyDictionary hashTags);

            if (hashTags != null)
            {
                var trendingHashTags = await Task.FromResult(hashTags.GetTopValuesWithCount(request.TopCount));
                return trendingHashTags;
            }
            else
                return new Dictionary<string, int>();
        }
    }
}
