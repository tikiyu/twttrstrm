using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Twitter.Stats.Application.Common.Models
{
    public class TweetStreamErrorDetails
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("detail")]
        public string Detail { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("status")]
        public int Status { get; set; }
    }
}
