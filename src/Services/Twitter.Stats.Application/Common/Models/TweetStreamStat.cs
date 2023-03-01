using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitter.Stats.Application.Common.Models
{
    public class TweetStreamStat
    {
        public int TotalTweets { get; set; }
        public int TweetsPerSecond { get; set; }
    }
}
