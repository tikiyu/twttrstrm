using MediatR;
using Microsoft.AspNetCore.Mvc;
using Twitter.Stats.Application.Common.Interfaces;
using Twitter.Stats.Application.Common.Models;
using Twitter.Stats.Application.Tweets.Queries;
using Twitter.Stats.Application.Tweets.Queries.GetPaginatedTweets;

namespace Twitter.Stats.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TweetsController : ControllerBase
    {
        private readonly ITweetRepository _repository;
        private readonly ILogger<TweetsController> _logger;
        private readonly IMediator _mediator;

        public TweetsController(ILogger<TweetsController> logger, ITweetRepository repository, IMediator mediator)
            => (_logger, _repository, _mediator) = (logger, repository, mediator);

        [HttpGet("recent")]
        public async Task<IEnumerable<TweetDto>> GetRecentAsync([FromQuery] GetTweetsQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Request start Get Recent Tweets");

            return await _mediator.Send(query, cancellationToken);

        }

        [HttpGet("page")]
        public async Task<ActionResult<PaginatedList<TweetDto>>> GetPaginatedTweetsAsync([FromQuery] GetPaginatedTweetsQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Request start Get Paginated Tweets");

            return await _mediator.Send(query, cancellationToken);
        }
    }
}