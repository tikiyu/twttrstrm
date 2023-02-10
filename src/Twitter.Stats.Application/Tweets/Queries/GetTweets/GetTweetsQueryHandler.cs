using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Twitter.Stats.Application.Common.Interfaces;

namespace Twitter.Stats.Application.Tweets.Queries
{
    public class GetTweetsQueryHandler : IRequestHandler<GetTweetsQuery, IEnumerable<TweetDto>>
    {
        private readonly ITweetRepository _repository;
        private readonly IMapper _mapper;

        public GetTweetsQueryHandler(ITweetRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TweetDto>> Handle(GetTweetsQuery request, CancellationToken cancellationToken)
        {
            var tweets = await _repository.GetTweetsAsync(cancellationToken);

            return tweets.AsQueryable().Where(x => x.CreatedAt > DateTime.Now.AddDays(-1))
                   .OrderBy(x => x.CreatedAt)
                   .ProjectTo<TweetDto>(_mapper.ConfigurationProvider)
                   .Take(request.Count);
        }
    }
}
