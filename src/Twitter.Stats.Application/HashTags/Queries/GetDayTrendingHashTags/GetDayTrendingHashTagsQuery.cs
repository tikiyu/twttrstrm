using MediatR;

namespace Twitter.Stats.Application.HashTags.Queries.GetDayTrendingHashTags
{
    public class GetDayTrendingHashTagsQuery : IRequest<Dictionary<string, int>>
    {
        public int TopCount { get; set; }
    }
}
