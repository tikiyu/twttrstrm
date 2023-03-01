using MediatR;

namespace Twitter.Stats.Application.Tweets.Queries
{
    public record GetTweetsQuery : IRequest<IEnumerable<TweetDto>>
    {
        public int Count { get; set; }
    }
}
