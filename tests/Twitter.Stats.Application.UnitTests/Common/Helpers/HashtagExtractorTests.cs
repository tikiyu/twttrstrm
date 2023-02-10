using Twitter.Stats.Application.Common.Helpers;

namespace Twitter.Stats.Application.UnitTests.Common.Helpers
{
    public class HashtagExtractorTests
    {
        [Theory]
        [InlineData("This is a sample #tweet with multiple hashtag #test for #xunit", new[] { "tweet", "test", "xunit" })]
        [InlineData("#singlehashtag", new[] { "singlehashtag" })]
        [InlineData("reversedhashtag#", new string[] { })]
        [InlineData("Sample tweet with No hashtags @mention", new string[] { })]
        [InlineData("Sample tweet number hash #123", new string[] { "123" })]
        [InlineData("Sample tweet symbols #åå±± #åå· #æ­é", new string[] { "åå±±", "åå·", "æ­é" })]
        [InlineData("Sample just # symbol", new string[] { })]
        public void ExtractHashtags_ReturnsExpectedHashtags(string text, string[] expectedHashtags)
        {
            // Act
            HashSet<string> hashtags = text.ExtractHashtags();

            // Assert
            Assert.Equal(expectedHashtags, hashtags);
        }
    }
}
