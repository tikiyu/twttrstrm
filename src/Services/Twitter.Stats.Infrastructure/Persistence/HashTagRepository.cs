using MongoDB.Driver;
using Twitter.Stats.Application.Common.Interfaces;
using Twitter.Stats.Domain.Entities;

namespace Twitter.Stats.Infrastructure.Persistence
{
    public class HashTagRepository : IHashTagRepository
    {
        private readonly IApplicationDb _applicationDb;
        private readonly SemaphoreSlim _semaphoreSlim;

        public HashTagRepository(IApplicationDb applicationDb) =>
            (_applicationDb, _semaphoreSlim) = (applicationDb, new SemaphoreSlim(1, Environment.ProcessorCount));


        public async Task<IList<HashTag>> GetHashTagsAsync(CancellationToken cancellationToken)
        {
            #region Aggregation pipeline not working on MongoDbInmemory
            //var startDate = DateTime.Now.AddDays(-rangeInDays);
            //var endDate = DateTime.Now;

            // Define the aggregation pipeline
            //            var pipeline = new BsonDocument[]
            //{
            //    new BsonDocument
            //    {
            //        { "$match", new BsonDocument
            //            {
            //                { "Date", new BsonDocument
            //                    {
            //                        { "$gte", new BsonDateTime(startDate) },
            //                        { "$lte", new BsonDateTime(endDate) }
            //                    }
            //                }
            //            }
            //        }
            //    },
            //    new BsonDocument
            //    {
            //        { "$group", new BsonDocument
            //            {
            //                { "_id", "$Tag" },
            //                { "count", new BsonDocument
            //                    {
            //                        { "$sum", 1 }
            //                    }
            //                }
            //            }
            //        }
            //    },
            //    new BsonDocument
            //    {
            //        { "$sort", new BsonDocument
            //            {
            //                { "count", -1 }
            //            }
            //        }
            //    }
            //};

            #endregion

            await _semaphoreSlim.WaitAsync();
            try
            {
                var filter = Builders<HashTag>.Filter.Ne("Id", string.Empty);//Bug on MongoDbInMemory not allowing Builders<Tweet>.Filter.Empty
                var result = await _applicationDb.HashTags.Find(filter).ToListAsync(cancellationToken: cancellationToken);
                //var result = await _applicationDb.HashTags.Aggregate<HashTag>(pipeline).ToListAsync();
                return result;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task InsertHashTagAsync(HashTag hashTag)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                await _applicationDb.HashTags.InsertOneAsync(hashTag);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public Task InsertHashTagAsync(HashTag tweet, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task InsertHashTagsAsync(IEnumerable<HashTag> hashTags, CancellationToken cancellationToken)
        {
            await _semaphoreSlim.WaitAsync(cancellationToken);
            try
            {
                await _applicationDb.HashTags.InsertManyAsync(hashTags, cancellationToken: cancellationToken);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}

