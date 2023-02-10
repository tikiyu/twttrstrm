using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Diagnostics;
using Tweetinvi;
using Twitter.Stats.Application.Common.Interfaces;
using Twitter.Stats.Application.Common.Models;
using Twitter.Stats.Application.Tweets.Commands.CreateTweet;
using Twitter.Stats.Infrastructure.Settings;


namespace Twitter.Stats.API.Services
{
    public sealed class StreamTwitterService : IStreamTwitterService
    {
        private int _tweetCount;
        private readonly ITwitterClient _twitterClient;
        private readonly ILogger<StreamTwitterService> _logger;
        private readonly IMediator _mediator;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SimulationSettings _simulationSettings;
        private readonly TwitterClientSettings _twitterClientSettings;

        public StreamTwitterService(
            ITwitterClient twitterClient,
            IMediator mediator,
            ILogger<StreamTwitterService> logger,
            IHttpClientFactory httpClientFactory,
            IOptions<SimulationSettings> simulationSettings,
            IOptions<TwitterClientSettings> twitterClientSettings) =>
            (_twitterClient, _logger, _httpClientFactory, _mediator, _simulationSettings, _twitterClientSettings)
            = (twitterClient, logger, httpClientFactory, mediator, simulationSettings.Value, twitterClientSettings.Value);

        public async Task DoWorkAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(StreamTwitterService)} is running.");

            Stopwatch stopWatch = new();
            stopWatch.Start();

            var httpClient = _httpClientFactory.CreateClient("TwitterClient");

            //using Stream stream = await httpClient.GetStreamAsync("/2/tweets/sample/stream", cancellationToken);

            //while (stream.CanRead)
            //{
            //    byte[] buffer = new byte[1024];
            //    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
            //    if (bytesRead > 0)
            //    {
            //        string receivedText = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            //        var tweet = JsonConvert.DeserializeObject<TweetStream>(receivedText);
            //        //Console.WriteLine("Stream text received: " + tweet?.Data.Text);
            //        _tweetCount++;
            //        DisplayLogs(stopWatch, tweet);
            //        //Console.WriteLine("Stream text received: " + receivedText);
            //    }
            //}

            int _tweetCount = 0;

            using var stream = await httpClient.GetStreamAsync(_twitterClientSettings.Endpoints.VolumeStream, cancellationToken);

            while (true)
            {

                var receivedTweet = ReadEvent(stream);
                var tweet = JsonConvert.DeserializeObject<TweetStream>(receivedTweet);
                if (tweet != null)
                {
                    await ExecuteSaveTweetsAsync(tweet, cancellationToken);
                    _tweetCount++;
                    #region Console Display
                    Console.SetCursorPosition(0, 0);
                    _logger.LogInformation($"Total Tweets Received: {_tweetCount * _simulationSettings.TweetReceivedMultiplier} - Processed Tweets per second: {(int)(_tweetCount * _simulationSettings.TweetReceivedMultiplier / stopWatch.Elapsed.TotalSeconds)}                                            ");
                    _logger.LogInformation($"Current Tweet: {tweet.Data.Text[..Math.Min(tweet.Data.Text.Length, 40)].Replace("\n", string.Empty)}...");
                    _logger.LogInformation($"=========================================================================================================");
                    #endregion

                }

            }
        }

        private static string ReadEvent(Stream stream)
        {
            var data = "";
            while (true)
            {
                var nextByte = stream.ReadByte();
                if (nextByte == '\n')
                {
                    break;
                }
                data += (char)nextByte;
            }
            return data;
        }

        private async Task ExecuteSaveTweetsAsync(TweetStream tweet, CancellationToken cancellationToken)
        {
            if (_simulationSettings.IsSimulation)
            {
                for (int i = 0; i < _simulationSettings.TweetReceivedMultiplier; i++)
                {
                    await _mediator.Send(new CreateTweetStreamCommand { Tweet = tweet }, cancellationToken).ConfigureAwait(false);
                }
            }
            else
            {
                await _mediator.Send(new CreateTweetStreamCommand { Tweet = tweet }, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
