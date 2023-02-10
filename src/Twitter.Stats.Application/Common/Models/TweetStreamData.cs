using System.Text.Json.Serialization;

namespace Twitter.Stats.Application.Common.Models
{
    public class TweetStream
    {
        public TweetStreamData Data { get; set; }
    }

    public class TweetStreamData
    {
        [JsonPropertyName("edit_history_tweet_ids")]
        public IEnumerable<string> EditHistoryTweetIds { get; set; }
        public string Id { get; set; }
        public string Text { get; set; }
    }
}
