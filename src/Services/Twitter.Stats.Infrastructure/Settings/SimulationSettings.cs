namespace Twitter.Stats.Infrastructure.Settings
{
    public class SimulationSettings
    {
        public int TweetReceivedMultiplier { get; set; }
        public bool IsSimulation { get; set; }
        public int MaxDegreeOfParallelism { get; set; }
    }
}
