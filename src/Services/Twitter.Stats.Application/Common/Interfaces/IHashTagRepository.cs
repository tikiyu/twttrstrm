using Twitter.Stats.Domain.Entities;

namespace Twitter.Stats.Application.Common.Interfaces
{
    public interface IHashTagRepository
    {
        Task<IList<HashTag>> GetHashTagsAsync(CancellationToken cancellationToken);
        Task InsertHashTagAsync(HashTag tweet, CancellationToken cancellationToken);
        Task InsertHashTagsAsync(IEnumerable<HashTag> hashTags, CancellationToken cancellationToken);
    }
}
