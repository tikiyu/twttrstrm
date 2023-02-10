namespace Twitter.Stats.Application.Common.Interfaces
{
    public interface IStreamTwitterProcessingService
    {
        Task DoWorkAsync(CancellationToken cancellationToken);
    }

}
