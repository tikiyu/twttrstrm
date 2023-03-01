using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twitter.Stats.Application.Common.Helpers;
using Twitter.Stats.Application.Common.Interfaces;
using Twitter.Stats.Application.Common.Settings;
using Twitter.Stats.Application.Tweets.Commands.CreateTweet;
using Twitter.Stats.Domain.Entities;

namespace Twitter.Stats.Application.HashTags.Commands
{
    public record  CreateHashTagCommand : IRequest
    {
        public IEnumerable<string> Tags { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }

    public class CreateHashTagCommandCommandHandler : IRequestHandler<CreateHashTagCommand, Unit>
    {
        private readonly IHashTagService _hashTagService;
        private readonly IMediator _mediator;
        private readonly IThreadSafeMemoryCache<string, FrequencyDictionary> _memoryCache;
        private readonly HashtagSettings _hashtagSettings;
        private readonly ParallelOptions _parallelOptions;

        public CreateHashTagCommandCommandHandler(
            IHashTagService hashTagService,
            IMediator mediator,
            IOptions<HashtagSettings> hashtagSettings,
             IThreadSafeMemoryCache<string, FrequencyDictionary> memoryCache)
        {
            _hashTagService = hashTagService;
            _mediator = mediator;
            _hashtagSettings = hashtagSettings.Value;
            _memoryCache = memoryCache;
            _parallelOptions = new()
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
            };
        }

        public async Task<Unit> Handle(CreateHashTagCommand request, CancellationToken cancellationToken)
        {

            IEnumerable<HashTag> hashTags = request.Tags.Select(tag => new HashTag
            {
                CreatedAt = request.CreatedAt,
                Tag = tag
            }).ToList();

            await _hashTagService.InsertHashTag(hashTags);

            return Unit.Value;
        }
    }
}
