namespace Twitter.Stats.Application.Common.Interfaces
{
    public interface IDayTrendingHashTagService
    {
        Task DoWorkAsync(CancellationToken cancellationToken);
    }
}
