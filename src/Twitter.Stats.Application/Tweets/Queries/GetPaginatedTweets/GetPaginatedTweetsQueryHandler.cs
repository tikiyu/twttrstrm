using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Twitter.Stats.Application.Common.Interfaces;
using Twitter.Stats.Application.Common.Models;
using Twitter.Stats.Application.Extensions;

namespace Twitter.Stats.Application.Tweets.Queries.GetPaginatedTweets
{
    public class GetPaginatedTweetsQueryHandler : IRequestHandler<GetPaginatedTweetsQuery, PaginatedList<TweetDto>>
    {
        private readonly ITweetRepository _repository;
        private readonly IMapper _mapper;

        public GetPaginatedTweetsQueryHandler(ITweetRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PaginatedList<TweetDto>> Handle(GetPaginatedTweetsQuery request, CancellationToken cancellationToken)
        {
            var tweets = await _repository.GetTweetsAsync(cancellationToken);

            return tweets.AsQueryable().Where(x => x.CreatedAt > DateTime.Now.AddDays(-1))
                   .OrderBy(x => x.CreatedAt)
                   .ProjectTo<TweetDto>(_mapper.ConfigurationProvider)
                   .PaginatedListAsync(request.PageNumber, request.PageSize);
        }
    }
}
