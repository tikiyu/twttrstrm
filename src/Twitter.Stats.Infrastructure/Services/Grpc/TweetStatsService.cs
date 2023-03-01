using Grpc.Net.ClientFactory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twitter.Stats.API;
using Twitter.Stats.Application.Common.Interfaces;
using Twitter.Stats.Application.Common.Models;

namespace Twitter.Stats.Infrastructure.Services.Grpc
{
    public class TweetStatsService : ITweetStatsService
    {
        private readonly API.TweetStatGrpc.TweetStatGrpcClient _client;
        private readonly ILogger<TweetStatsService> _logger;

        public TweetStatsService(GrpcClientFactory grpcClientFactory, ILogger<TweetStatsService> logger)
        {
            _logger = logger;
            _client = grpcClientFactory.CreateClient<API.TweetStatGrpc.TweetStatGrpcClient>("TweetStatGrpc");
        }

        public async Task SendTweetStatAsync(TweetStreamStat tweetStreamStat)
        {
            try
            {
                var request = new TweetStatRequest
                {
                    TotalTweets = tweetStreamStat.TotalTweets,
                    TweetsPerSecond = tweetStreamStat.TweetsPerSecond
                };

                await _client.SendTweetStatAsync(request);

            }
            catch (Exception ex)
            {
                _logger.LogError(message: "Error:{ex}", ex.ToString());

#if DEBUG
                using (FileStream fs = new($"{nameof(TweetStatsService)}.error.log", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    StreamWriter write = new(fs);
                    write.BaseStream.Seek(0, SeekOrigin.End);
                    write.WriteLine($"{DateTime.Now}:{ex}");
                    write.WriteLine(Environment.NewLine);

                    write.Flush();
                    write.Close();
                    fs.Close();
                }
#endif
            }


        }
    }
}
