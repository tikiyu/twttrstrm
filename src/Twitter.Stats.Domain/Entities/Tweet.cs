using Newtonsoft.Json;
using System.Text.Json;
using Twitter.Stats.Domain.Common;

namespace Twitter.Stats.Domain.Entities
{
    public class Tweet : BaseEntity
    {
        [JsonProperty("id")]
        public string TweetId { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("author_id")]
        public string AuthorId { get; set; }

        [JsonProperty("conversation_id")]
        public string ConversationId { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("hashtags")]
        public IEnumerable<string> Hashtags { get; set; }

        [JsonProperty("mentions")]
        public IEnumerable<string> Mentions { get; set; }


    }
}
