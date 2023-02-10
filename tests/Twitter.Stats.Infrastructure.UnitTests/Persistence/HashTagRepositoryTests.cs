using MongoDB.Bson;
using MongoDB.Driver;
using Twitter.Stats.Application.Common.Interfaces;
using Twitter.Stats.Domain.Entities;
using Twitter.Stats.Infrastructure.Persistence;

namespace Twitter.Stats.Infrastructure.UnitTests
{
    public class HashTagRepositoryTests
    {
        private IApplicationDb _applicationDb;
        private HashTagRepository _hashTagRepository;

        public HashTagRepositoryTests()
        {
            _applicationDb = Substitute.For<IApplicationDb>();
            _hashTagRepository = new HashTagRepository(_applicationDb);
        }

        [Fact]
        public async Task InsertHashTagAsync_Should_Insert_HashTag()
        {
            // Arrange
            var hashTag = new AutoFaker<HashTag>()
                .RuleFor(h => h.Id, f => ObjectId.GenerateNewId())
                .Generate();

            // Act
            await _hashTagRepository.InsertHashTagAsync(hashTag);

            // Assert
            await _applicationDb.HashTags.Received().InsertOneAsync(hashTag);
        }

        [Fact]
        public async Task InsertHashTagsAsync_Should_Insert_HashTags()
        {
            // Arrange
            var hashTags = new AutoFaker<HashTag>()
                .RuleFor(h => h.Id, f => ObjectId.GenerateNewId())
                .Generate(100);

            // Act
            await _hashTagRepository.InsertHashTagsAsync(hashTags, CancellationToken.None);

            // Assert
            await _applicationDb.HashTags.Received().InsertManyAsync(hashTags, Arg.Any<InsertManyOptions>(), CancellationToken.None);
        }
    }
}
