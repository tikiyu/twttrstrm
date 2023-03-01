using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using System.Reflection.Metadata.Ecma335;
using Twitter.Web.Settings;

namespace Twitter.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<TwitterStatsApiClientSettings>(configuration.GetSection(nameof(TwitterStatsApiClientSettings)));

            var retryPolicy = HttpPolicyExtensions
                                .HandleTransientHttpError()
                                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            services.AddHttpClient("TwitterStatsApiClient", (provider, httpClient) =>
            {
                var option = provider.GetService<IOptions<TwitterStatsApiClientSettings>>()?.Value;
                if (option != null)
                {
                    httpClient.BaseAddress = new Uri(option.BaseAddress);
                }
            }).AddPolicyHandler(retryPolicy);

            return services;
        }
    }
}
