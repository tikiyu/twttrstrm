using MediatR;

namespace Twitter.Stats.Application.HashTags.Queries.GetTrendingHashTags
{
    public class GetTrendingHashTagsQuery : IRequest<Dictionary<string, int>>
    {
        public int TopCount { get; set; }
    }

}
