using MediatR;
using Twitter.Stats.Application.Common.Models;

namespace Twitter.Stats.Application.Tweets.Commands.CreateTweet
{
    public class CreateTweetStreamCommand : IRequest
    {
        public TweetStream Tweet { get; set; }
    }
}
