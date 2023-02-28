using MediatR;
using Twitter.Stats.Application.Common.Models;

namespace Twitter.Stats.Application.Tweets.Queries.GetPaginatedTweets
{
    public record GetPaginatedTweetsQuery : IRequest<PaginatedList<TweetDto>>
    {
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;
    }
}
