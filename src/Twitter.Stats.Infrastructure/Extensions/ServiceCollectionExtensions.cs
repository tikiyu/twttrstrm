using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Net.Http;
using Twitter.Stats.API.Services;
using Twitter.Stats.Application.Common.Helpers;
using Twitter.Stats.Application.Common.Interfaces;
using Twitter.Stats.Infrastructure.Cache;
using Twitter.Stats.Infrastructure.Persistence;
using Twitter.Stats.Infrastructure.Services;
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

            services.AddScoped<IStreamTwitterProcessingService, StreamTwitterProcessingService>();
            services.AddScoped<IStreamTwitterService, StreamTwitterService>();
            services.AddScoped<IDayTrendingHashTagService, DayTrendingHashTagService>();

            services.AddScoped<IHashTagService, HashTagService>();


            services.AddGrpcClient<Hashtag.Service.HashTagGrpc.HashTagGrpcClient>("HashTagGrpcClient", (provider, o) =>
            {
                var option = provider.GetService<IOptions<GrpcClientSettings>>()?.Value;
                if (option != null)
                {
                    o.Address = new Uri(option.Uri);
                }
            });


            return services;
        }
    }
}

