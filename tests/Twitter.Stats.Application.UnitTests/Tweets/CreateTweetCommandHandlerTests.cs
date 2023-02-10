using MediatR;
using MongoDB.Bson;
using Twitter.Stats.Application.Common.Interfaces;
using Twitter.Stats.Application.Tweets.Commands.CreateTweet;
using Twitter.Stats.Domain.Entities;

namespace Twitter.Stats.Application.UnitTests.Tweets
{
    public class CreateTweetCommandHandlerTests
    {
        private readonly ITweetRepository _repository;
        private readonly IMediator _mediator;
        private readonly CreateTweetCommandHandler _handler;

        public CreateTweetCommandHandlerTests()
        {
            _repository = Substitute.For<ITweetRepository>();
            _mediator = Substitute.For<IMediator>();
            _handler = new CreateTweetCommandHandler(_repository, _mediator);
        }

        [Fact]
        public async Task Handle_WithNullCommand_ShouldReturnUnit()
        {
            // Arrange
            CreateTweetCommand command = AutoFaker.Generate<CreateTweetCommand>();
            Tweet tweetEntity = new AutoFaker<Tweet>()
                .RuleFor(fake => fake.Id, fake => ObjectId.GenerateNewId())
                .Generate();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(Unit.Value, result);
            //await _repository.Received().InsertTweetAsync(Arg.Is<>(tweetEntity), CancellationToken.None);
            //await _mediator.Received().Publish(Arg.Any<Tweet>(), Arg.Any<CancellationToken>());

        }
    }
}


