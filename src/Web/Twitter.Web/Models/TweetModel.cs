using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Twitter.Web.Models
{
    public class TweetModel { 
        public string Id { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public string TweetId { get; set; }

        public string Text { get; set; }

        public string AuthorId { get; set; }

    }
}
