using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Twitter.Stats.Domain.Entities;

namespace Twitter.Stats.Application.HashTags.Queries.GetDayTrendingHashTags
{
    public class GetDayTrendingHashTagsQueryHandler : IRequestHandler<GetDayTrendingHashTagsQuery, Dictionary<string, int>>
    {
        private readonly IMemoryCache _memoryCache;

        public GetDayTrendingHashTagsQueryHandler(IMemoryCache memoryCache, IMediator mediator)
        {
            _memoryCache = memoryCache;
        }

        public async Task<Dictionary<string, int>> Handle(GetDayTrendingHashTagsQuery request, CancellationToken cancellationToken)
        {
            _memoryCache.TryGetValue<Dictionary<string, int>>($"Day{nameof(HashTag)}", out var value);

            return await Task.FromResult(value ?? new Dictionary<string, int>());
        }

    }
}
