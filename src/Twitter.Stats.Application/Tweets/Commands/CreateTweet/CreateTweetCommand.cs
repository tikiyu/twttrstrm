using MediatR;
using Tweetinvi.Models.V2;

namespace Twitter.Stats.Application.Tweets.Commands.CreateTweet
{
    public class CreateTweetCommand : IRequest
    {
        public TweetV2 Tweet { get; set; }
    }
}
