using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Polly;
using Polly.Extensions.Http;
using Tweetinvi;
using Tweetinvi.Models;
using Twitter.Stats.API.Hubs;
using Twitter.Stats.API.Jobs;
using Twitter.Stats.Application.Common.Settings;
using Twitter.Stats.Infrastructure.Settings;

namespace Twitter.Stats.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<TwitterClientSettings>(configuration.GetSection(nameof(TwitterClientSettings)));
            services.Configure<MongoDbSettings>(configuration.GetSection(nameof(MongoDbSettings)));
            services.Configure<HashtagSettings>(configuration.GetSection(nameof(HashtagSettings)));
            services.Configure<SimulationSettings>(configuration.GetSection(nameof(SimulationSettings)));
            services.Configure<GrpcClientSettings>(configuration.GetSection(nameof(GrpcClientSettings)));
            

            services.AddHostedService<StreamTwitterProcessingJob>();
            //services.AddHostedService<StreamTweetsJob>();
            services.AddSingleton<TweetStatsHub>();

            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            services.AddHttpClient("TwitterClient", (provider, httpClient) =>
            {
                var option = provider.GetService<IOptions<TwitterClientSettings>>()?.Value;
                if (option != null)
                {
                    httpClient.BaseAddress = new Uri(option.BaseAddress);
                    httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, option.Headers.Accept);
                    httpClient.DefaultRequestHeaders.Add(HeaderNames.Authorization, $"Bearer {option.Secrets.Token}");
                }
            }).AddPolicyHandler(retryPolicy);

            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                    .WithOrigins("https://localhost:44439")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });



            return services;
        }
    }
}
