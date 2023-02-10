using Twitter.Stats.Domain.Common;
using Twitter.Stats.Domain.Entities;

namespace Twitter.Stats.Domain.Events
{
    public class TweetCreatedEvent : BaseEvent
    {
        public TweetCreatedEvent(Tweet tweet)
        {
            Tweet = tweet;
        }

        public Tweet Tweet { get; }
    }
}
