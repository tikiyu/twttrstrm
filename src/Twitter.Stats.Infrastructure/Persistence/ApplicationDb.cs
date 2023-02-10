using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.InMemory;
using Twitter.Stats.Application.Common.Interfaces;
using Twitter.Stats.Domain.Entities;
using Twitter.Stats.Infrastructure.Settings;

namespace Twitter.Stats.Infrastructure.Persistence
{

    public class ApplicationDb : IApplicationDb
    {
        public IMongoDatabase Database { get; }

        public IMongoCollection<Tweet> Tweets { get; }

        public IMongoCollection<HashTag> HashTags { get; }

        private readonly IMediator _mediator;

        public ApplicationDb(IMediator mediator, IOptions<MongoDbSettings> settings)
        {
            _mediator = mediator;

            //Create In-Memory MongoDb Instance
            var client = InMemoryClient.Create();

            //Get mongo db related settings
            var mongoDbSettings = settings.Value;

            //Set Database
            Database = client.GetDatabase(mongoDbSettings.DatabaseName);

            //Set Collections
            Tweets = Database.GetCollection<Tweet>(mongoDbSettings.Collections.Tweets);
            HashTags = Database.GetCollection<HashTag>(mongoDbSettings.Collections.HashTags);
        }
    }
}

