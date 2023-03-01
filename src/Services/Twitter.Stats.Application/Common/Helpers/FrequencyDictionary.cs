using System.Collections.Concurrent;

namespace Twitter.Stats.Application.Common.Helpers
{
    public class FrequencyDictionary
    {
        private readonly ConcurrentDictionary<string, (int frequency, DateTime lastAccessed)> frequencyDictionary;
        private readonly TimeSpan timeToLive;

        public FrequencyDictionary(TimeSpan timeToLive)
        {
            if (timeToLive <= TimeSpan.Zero)
            {
                throw new ArgumentException("Time to live must be greater than zero", nameof(timeToLive));
            }

            this.timeToLive = timeToLive;
            frequencyDictionary = new ConcurrentDictionary<string, (int frequency, DateTime lastAccessed)>();
        }

        public void Add(string item)
        {

            if (frequencyDictionary.TryGetValue(item, out var value))
            {
                frequencyDictionary[item] = (frequencyDictionary[item].frequency + 1, DateTime.Now);
            }
            else
            {
                frequencyDictionary[item] = (1, DateTime.Now);
            }

            //if (frequencyDictionary.ContainsKey(item))
            //{
            //    frequencyDictionary[item] = (frequencyDictionary[item].frequency + 1, DateTime.Now);
            //}
            //else
            //{
            //    frequencyDictionary[item] = (1, DateTime.Now);
            //}

            Cleanup();
        }

        public void AddRange(IEnumerable<string> items)
        {
            foreach (var item in items)
            {
                if (frequencyDictionary.TryGetValue(item, out var value))
                {
                    frequencyDictionary[item] = (frequencyDictionary[item].frequency + 1, DateTime.Now);
                }
                else
                {
                    frequencyDictionary[item] = (1, DateTime.Now);
                }
                //if (frequencyDictionary.ContainsKey(item))
                //{
                //    frequencyDictionary[item] = (frequencyDictionary[item].frequency + 1, DateTime.Now);
                //}
                //else
                //{
                //    frequencyDictionary[item] = (1, DateTime.Now);
                //}
            }
            Cleanup();
        }

        public Dictionary<string, int> GetTopValuesWithCount(int count)
        {
            if (count <= 0)
            {
                throw new ArgumentException("Count must be greater than zero", nameof(count));
            }

            Cleanup();

            if (frequencyDictionary.IsEmpty)
            {
                return new Dictionary<string, int>();
            }

            var items = frequencyDictionary.ToArray();
            Array.Sort(items, (x, y) => y.Value.frequency.CompareTo(x.Value.frequency));

            Dictionary<string, int> topValues = new();

            foreach (var item in items.Take(Math.Min(count, items.Length)))
            {
                topValues.Add(item.Key, item.Value.frequency);
            }
            return topValues;
        }

        private void Cleanup()
        {
            var expiredItems = frequencyDictionary.Where(x => DateTime.Now - x.Value.lastAccessed > timeToLive).ToArray();

            foreach (var expiredItem in expiredItems)
            {
                frequencyDictionary.TryRemove(expiredItem.Key, out _);
            }
        }

    }
}

