using AutoMapper;
using Twitter.Stats.Application.Common.Mappings;
using Twitter.Stats.Domain.Entities;

namespace Twitter.Stats.Application.Tweets.Queries
{
    public class TweetDto : IMapFrom<Tweet>
    {
        public string Id { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public string TweetId { get; set; }

        public string Text { get; set; }

        public string AuthorId { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Tweet, TweetDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id.ToString()));
        }

    }
}
