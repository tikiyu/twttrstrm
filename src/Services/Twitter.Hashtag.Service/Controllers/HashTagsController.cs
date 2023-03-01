using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Twitter.Hashtag.Service.Hubs;
using Twitter.Stats.Application.HashTags.Queries.GetDayTrendingHashTags;
using Twitter.Stats.Application.HashTags.Queries.GetTrendingHashTags;

namespace Twitter.Hashtag.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HashTagsController : ControllerBase
    {
        private readonly ILogger<HashTagsController> _logger;
        private readonly IMediator _mediator;
        public HashTagsController(IMediator mediator, ILogger<HashTagsController> logger) =>
            (_mediator, _logger) = (mediator, logger);

         
        /// <summary>
        /// Gets the hot trending hashtag within the day from a cache(DayHashTag).
        /// cache(DayHashTag)is populated by DayTrendingHashTagCronJob.cs  
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("trending/day")]
        public async Task<ActionResult<Dictionary<string, int>>> GetDayTrendingAsync([FromQuery] GetDayTrendingHashTagsQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Request start Get Trending in A day");

            return await _mediator.Send(query, cancellationToken);
        }
    }
}
