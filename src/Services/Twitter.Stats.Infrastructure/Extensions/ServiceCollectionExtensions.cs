﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Net.Http;
using Tweetinvi.Models;
using Tweetinvi;
using Twitter.Stats.API.Services;
using Twitter.Stats.Application.Common.Helpers;
using Twitter.Stats.Application.Common.Interfaces;
using Twitter.Stats.Infrastructure.Cache;
using Twitter.Stats.Infrastructure.Persistence;
using Twitter.Stats.Infrastructure.Services;
using Twitter.Stats.Infrastructure.Services.Grpc;
using Twitter.Stats.Infrastructure.Settings;

namespace Twitter.Stats.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IApplicationDb, ApplicationDb>();
            services.AddSingleton<ITweetRepository, TweetRepository>();
            services.AddSingleton<IHashTagRepository, HashTagRepository>();
            services.AddSingleton<IThreadSafeMemoryCache<string, FrequencyDictionary>, ThreadSafeMemoryCache<string, FrequencyDictionary>>();
            services.AddSingleton<IStreamStatsService, StreamStatsService>();

            services.AddScoped<IStreamTwitterProcessingService, StreamTwitterProcessingService>();
            services.AddScoped<IStreamTwitterService, StreamTwitterService>();
            services.AddScoped<IDayTrendingHashTagService, DayTrendingHashTagService>();

            services.AddScoped<IHashTagService, HashTagService>();

            services.AddScoped<ITwitterClient>(provider =>
            {
                var option = provider.GetService<IOptions<TwitterClientSettings>>()?.Value;

                var appCredentials = new ConsumerOnlyCredentials(option?.Secrets.ConsumerKey, option?.Secrets.ConsumerSecret)
                {
                    BearerToken = option?.Secrets.Token
                };

                var twitterClient = new TwitterClient(appCredentials);
                return twitterClient;

            });

            services.AddGrpcClient<Hashtag.Service.HashTagGrpc.HashTagGrpcClient>("HashTagGrpcClient", (provider, o) =>
            {
                var option = provider.GetService<IOptions<GrpcClientSettings>>()?.Value;
                if (option != null)
                {
                    o.Address = new Uri(option.HashTagServiceUri);
                }
            });


            return services;
        }
    }
}


