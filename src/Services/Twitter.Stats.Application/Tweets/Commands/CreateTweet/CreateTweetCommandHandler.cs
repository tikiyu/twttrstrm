using MediatR;
using Twitter.Stats.Application.Common.Interfaces;
using Twitter.Stats.Domain.Entities;

namespace Twitter.Stats.Application.Tweets.Commands.CreateTweet
{
    public class CreateTweetCommandHandler : IRequestHandler<CreateTweetCommand, Unit>
    {
        private readonly ITweetRepository _repository;
        private readonly IMediator _mediator;
        public CreateTweetCommandHandler(ITweetRepository repository, IMediator mediator)
        {
            _repository = repository;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(CreateTweetCommand request, CancellationToken cancellationToken)
        {
            if (request == null || request.Tweet == null) return Unit.Value;

            var tweet = request.Tweet;

            var entity = new Tweet
            {
                Text = tweet.Text,
                CreatedAt = tweet.CreatedAt,
                AuthorId = tweet.AuthorId,
                Source = tweet.Source,
                ConversationId = tweet.ConversationId,
                TweetId = tweet.Id,
                Hashtags = tweet.Entities.Hashtags?.Select(x => x.Tag) ?? Array.Empty<string>(),
                Mentions = tweet.Entities.Mentions?.Select(x => x.Username) ?? Array.Empty<string>()
            };

            await _repository.InsertTweetAsync(entity, cancellationToken).ConfigureAwait(false);

            await _mediator.Publish(new Domain.Events.TweetCreatedEvent(entity), cancellationToken);

            return Unit.Value;

        }
    }
}
