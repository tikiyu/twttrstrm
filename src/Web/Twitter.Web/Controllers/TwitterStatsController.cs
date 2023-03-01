using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading;
using Twitter.Web.Models;

namespace Twitter.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TwitterStatsController : ControllerBase
    {

        private readonly ILogger<TwitterStatsController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        public TwitterStatsController(ILogger<TwitterStatsController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IEnumerable<TweetModel>> Get([FromQuery]int count, CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient("TwitterStatsApiClient");

            var recentTweetsResponse = await httpClient.GetAsync($"api/Tweets/recent?Count={count}", cancellationToken);

            Task<IEnumerable<TweetModel>?> recentTweets = recentTweetsResponse.Content.ReadFromJsonAsync<IEnumerable<TweetModel>>(cancellationToken: cancellationToken);

            return await recentTweets;
        }
    }
}