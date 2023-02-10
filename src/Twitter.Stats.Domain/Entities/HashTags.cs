using Newtonsoft.Json;
using Twitter.Stats.Domain.Common;

namespace Twitter.Stats.Domain.Entities
{
    public class HashTag : BaseEntity
    {
        [JsonProperty("tag")]
        public string Tag { get; set; }

    }
}
