using Twitter.Stats.Application.Common.Helpers;

namespace Twitter.Stats.Application.UnitTests
{
    namespace FrequencyDictionaryTest
    {
        public class FrequencyDictionaryTests
        {
            [Fact]
            public void TimeToLive_MustBe_GreaterThanZero()
            {
                //Assert
                Assert.Throws<ArgumentException>(() => new FrequencyDictionary(TimeSpan.Zero));
            }

            [Fact]
            public void Count_MustBe_GreaterThanZero()
            {
                //Arrange
                var dictionary = new FrequencyDictionary(TimeSpan.FromSeconds(1));

                //Assert
                Assert.Throws<ArgumentException>(() => dictionary.GetTopValuesWithCount(0));
            }

            [Theory]
            [InlineData(10000, 10)]
            public async Task Concurreny_AddAndGet_TopValues_WithCount(int count, int top)
            {
                //Arrange
                var dictionary = new FrequencyDictionary(TimeSpan.FromSeconds(1));
                var hashTagsFaker = AutoFaker.Generate<string>(count);

                //Act 
                ParallelOptions parallelOptions = new()
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount,
                };

                Parallel.ForEach(hashTagsFaker, parallelOptions, dictionary.Add);

                //foreach (var hashTag in hashTagsFaker)
                //{
                //    dictionary.Add(hashTag);
                //}

                var topValues = dictionary.GetTopValuesWithCount(top);

                //Assert
                Assert.Equal(top, topValues.Count);
                Assert.True(topValues.First().Value > topValues.Last().Value);
            }

            [Theory]
            [InlineData(1000, 10)]
            [InlineData(10000, 10)]
            [InlineData(100000, 10)]
            public void AddRangeAndGet_TopValues_WithCount(int count, int top)
            {
                //Arrange
                var dictionary = new FrequencyDictionary(TimeSpan.FromSeconds(1));
                var hashTagsFaker = AutoFaker.Generate<string>(count);

                //Act
                dictionary.AddRange(hashTagsFaker);
                var topValues = dictionary.GetTopValuesWithCount(top);

                //Assert
                Assert.Equal(top, topValues.Count);
                Assert.True(topValues.First().Value > topValues.Last().Value);
            }


        }
    }
}