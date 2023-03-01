
namespace Twitter.Stats.Application.Common.Settings
{
    public class HashtagSettings
    {
        public int TrendingTtlInMins { get; set; }
        public string DayTrendingCronJobSchedule { get; set; }
        public int DayTrendingCronJobDelayInSecs { get; set; }
        public int DayTrendingCount { get; set; }
        public int TrendingPushIntervalInMs { get; set; }
        public int TopTrendingHashtagCount { get; set; }

    }
}
