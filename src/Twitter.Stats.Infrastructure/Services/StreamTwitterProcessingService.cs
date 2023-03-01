using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text.Json;
using Tweetinvi;
using Tweetinvi.Models.V2;
using Twitter.Stats.Application.Common.Interfaces;
using Twitter.Stats.Application.Common.Models;
using Twitter.Stats.Application.Tweets.Commands.CreateTweet;
using Twitter.Stats.Infrastructure.Services;
using Twitter.Stats.Infrastructure.Settings;

namespace Twitter.Stats.API.Services
{
    public sealed class StreamTwitterProcessingService : IStreamTwitterProcessingService
    {
        private int _tweetCount;
        private readonly ITwitterClient _twitterClient;
        private readonly ILogger<StreamTwitterProcessingService> _logger;
        private readonly IMediator _mediator;
        private readonly SimulationSettings _simulationSettings;
        private readonly IStreamStatsService _streamStatsService;
        public StreamTwitterProcessingService(
            ITwitterClient twitterClient,
            IMediator mediator,
            ILogger<StreamTwitterProcessingService> logger,
            IOptions<SimulationSettings> simulationSettings,
            IStreamStatsService streamStatsService) =>
            (_twitterClient, _logger, _mediator, _simulationSettings, _streamStatsService)
            = (twitterClient, logger, mediator, simulationSettings.Value, streamStatsService);

        public async Task DoWorkAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(StreamTwitterProcessingService)} is running.");

            Stopwatch stopWatch = new();
            stopWatch.Start();
            var batchTweet = new List<TweetV2>();


            while (!cancellationToken.IsCancellationRequested)
            {
                var sampleStreamV2 = _twitterClient.StreamsV2.CreateSampleStream();

                sampleStreamV2.TweetReceived += async (sender, args) =>
                {
                    if (args.Tweet != null)
                    {
                        await ExecuteSaveTweetsAsync(args.Tweet, cancellationToken);

                        _tweetCount++;

                        _streamStatsService.UpdateStat(new TweetStreamStat
                        {
                            TotalTweets = _tweetCount * _simulationSettings.TweetReceivedMultiplier,
                            TweetsPerSecond = (int)(_tweetCount * _simulationSettings.TweetReceivedMultiplier / stopWatch.Elapsed.TotalSeconds)
                        });
                    }
                    else {
                        var errorResponse = JsonSerializer.Deserialize<TweetStreamErrorDetails>(args.Json);
                        _logger.LogCritical("Title: {title} - Detail: {detail} - StatusCode: {statusCode}", errorResponse?.Title, errorResponse?.Detail, errorResponse?.Status);
                        throw new HttpRequestException(errorResponse?.Title);
                    }
                };
                await sampleStreamV2.StartAsync();
            }
        }

        //private void DisplayLogs(Stopwatch stopWatch, TweetV2 tweet)
        //{

        //    Console.SetCursorPosition(0, 0);
        //    Console.Write(new string(' ', Console.WindowWidth));
        //    Console.WriteLine($"Total Tweets Received: {_tweetCount * _simulationSettings.TweetReceivedMultiplier} - Processed Tweets per second: {(int)(_tweetCount * _simulationSettings.TweetReceivedMultiplier / stopWatch.Elapsed.TotalSeconds)}                                            ");
        //    Console.Write(new string(' ', Console.WindowWidth));
        //    Console.WriteLine($"Current Tweet: {tweet.Text[..Math.Min(tweet.Text.Length, 40)].Replace("\n", string.Empty)}...             ");
        //    Console.Write(new string(' ', Console.WindowWidth));
        //    Console.WriteLine($"=========================================================================================================");
        //    Console.Write(new string(' ', Console.WindowWidth));
        //    Console.SetCursorPosition(0, 7);
        //}

        private async Task ExecuteSaveTweetsAsync(TweetV2 tweet, CancellationToken cancellationToken)
        {
            if (_simulationSettings.IsSimulation)
            {
                for (int i = 0; i < _simulationSettings.TweetReceivedMultiplier; i++)
                {
                    await _mediator.Send(new CreateTweetCommand { Tweet = tweet }, cancellationToken).ConfigureAwait(false);
                }
            }
            else
            {
                await _mediator.Send(new CreateTweetCommand { Tweet = tweet }, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
