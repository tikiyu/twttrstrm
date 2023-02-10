using MediatR;
using Twitter.Stats.Application.Common.Helpers;
using Twitter.Stats.Application.Common.Interfaces;
using Twitter.Stats.Domain.Entities;

namespace Twitter.Stats.Application.Tweets.Commands.CreateTweet
{
    public class CreateTweetStreamCommandHandler : IRequestHandler<CreateTweetStreamCommand, Unit>
    {
        private readonly ITweetRepository _repository;
        private readonly IMediator _mediator;
        public CreateTweetStreamCommandHandler(ITweetRepository repository, IMediator mediator)
        {
            _repository = repository;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(CreateTweetStreamCommand request, CancellationToken cancellationToken)
        {
            if (request == null) return Unit.Value;

            var tweet = request.Tweet;

            var entity = new Tweet
            {
                Text = tweet.Data.Text,
                CreatedAt = DateTime.Now,
                Hashtags = tweet.Data.Text.ExtractHashtags(),
            };

            await _repository.InsertTweetAsync(entity, cancellationToken).ConfigureAwait(false);

            await _mediator.Publish(new Domain.Events.TweetCreatedEvent(entity), cancellationToken);

            return Unit.Value;

        }
    }
}
