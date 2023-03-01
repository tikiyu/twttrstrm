namespace Twitter.Stats.Application.Common.Interfaces
{
    public interface IStreamTwitterService
    {
        Task DoWorkAsync(CancellationToken cancellationToken);
    }

}
