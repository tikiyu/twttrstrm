using MongoDB.Driver;
using Twitter.Stats.Application.Common.Interfaces;
using Tweet = Twitter.Stats.Domain.Entities.Tweet;

namespace Twitter.Stats.Infrastructure.Persistence
{
    public class TweetRepository : ITweetRepository
    {
        private readonly IApplicationDb _applicationDb;
        private readonly SemaphoreSlim _semaphoreSlim;

        public TweetRepository(IApplicationDb applicationDb) =>
            (_applicationDb, _semaphoreSlim) = (applicationDb, new SemaphoreSlim(1, Environment.ProcessorCount));


        public async Task<IList<Tweet>> GetTweetsAsync(CancellationToken cancellationToken)
        {
            var filter = Builders<Tweet>.Filter.Ne("Id", string.Empty);//Bug on MongoDbInMemory not allowing Builders<Tweet>.Filter.Empty
            var sort = Builders<Tweet>.Sort.Descending("created_At");

            await _semaphoreSlim.WaitAsync(cancellationToken);
            try
            {
                var result = await _applicationDb.Tweets.Find(filter).Sort(sort).ToListAsync(cancellationToken);
                return result;
            }
            finally
            {
                _semaphoreSlim.Release();
            }

        }

        public async Task<Tweet> GetTweetsByIdAsync(string Id, CancellationToken cancellationToken)
        {
            var filter = Builders<Tweet>.Filter.Eq("Id", Id);

            await _semaphoreSlim.WaitAsync(cancellationToken);

            try
            {
                var result = _applicationDb.Tweets.Find(filter).FirstOrDefault(cancellationToken: cancellationToken);
                return result;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task<long> GetTweetsCountAsync(CancellationToken cancellationToken)
        {
            var filter = Builders<Tweet>.Filter.Ne("Id", string.Empty);//Bug on MongoDbInMemory not allowing Builders<Tweet>.Filter.Empty

            await _semaphoreSlim.WaitAsync(cancellationToken);

            try
            {
                var result = _applicationDb.Tweets.CountDocuments(filter, cancellationToken: cancellationToken);
                return result;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task InsertTweetAsync(Tweet tweet, CancellationToken cancellationToken)
        {
            await _semaphoreSlim.WaitAsync(cancellationToken);
            try
            {
                await _applicationDb.Tweets.InsertOneAsync(tweet, cancellationToken: cancellationToken);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task InsertTweetsAsync(IEnumerable<Tweet> tweets, CancellationToken cancellationToken)
        {
            await _semaphoreSlim.WaitAsync(cancellationToken);
            try
            {
                await _applicationDb.Tweets.InsertManyAsync(tweets, cancellationToken: cancellationToken);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

    }
}
