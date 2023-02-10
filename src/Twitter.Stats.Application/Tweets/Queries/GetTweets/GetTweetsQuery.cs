using MediatR;

namespace Twitter.Stats.Application.Tweets.Queries
{
    public class GetTweetsQuery : IRequest<IEnumerable<TweetDto>>
    {
        public int Count { get; set; }
    }
}
