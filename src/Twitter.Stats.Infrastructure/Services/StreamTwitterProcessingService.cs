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

        public StreamTwitterProcessingService(
            ITwitterClient twitterClient,
            IMediator mediator,
            ILogger<StreamTwitterProcessingService> logger,
            IOptions<SimulationSettings> simulationSettings) =>
            (_twitterClient, _logger, _mediator, _simulationSettings)
            = (twitterClient, logger, mediator, simulationSettings.Value);

        public async Task DoWorkAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(StreamTwitterProcessingService)} is running.");

            Stopwatch stopWatch = new();
            stopWatch.Start();
            var batchTweet = new List<TweetV2>();

            ParallelOptions parallelOptions = new()
            {
                MaxDegreeOfParallelism = _simulationSettings.MaxDegreeOfParallelism
            };

            while (!cancellationToken.IsCancellationRequested)
            {
                var sampleStreamV2 = _twitterClient.StreamsV2.CreateSampleStream();

                sampleStreamV2.TweetReceived += async (sender, args) =>
                {
                    if (args.Tweet != null)
                    {

                        batchTweet.Add(args.Tweet);

                        _tweetCount++;
               
                        if (_tweetCount % _simulationSettings.MaxDegreeOfParallelism == 0) {
                            var newList = batchTweet.ToList();
                            await Parallel.ForEachAsync(newList, parallelOptions, async (tweet,cancellationToken) => {

                               await ExecuteSaveTweetsAsync(tweet, cancellationToken);
                           });

                            batchTweet.Clear();
                        }


                        DisplayLogs(stopWatch, args.Tweet);
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

        private void DisplayLogs(Stopwatch stopWatch, TweetV2 tweet)
        {
            Console.SetCursorPosition(0, 0);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.WriteLine($"Total Tweets Received: {_tweetCount * _simulationSettings.TweetReceivedMultiplier} - Processed Tweets per second: {(int)(_tweetCount * _simulationSettings.TweetReceivedMultiplier / stopWatch.Elapsed.TotalSeconds)}                                            ");
            Console.Write(new string(' ', Console.WindowWidth));
            Console.WriteLine($"Current Tweet: {tweet.Text[..Math.Min(tweet.Text.Length, 40)].Replace("\n", string.Empty)}...             ");
            Console.Write(new string(' ', Console.WindowWidth));
            Console.WriteLine($"=========================================================================================================");
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, 7);
        }

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
