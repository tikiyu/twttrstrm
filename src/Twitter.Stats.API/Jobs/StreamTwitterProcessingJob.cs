using Twitter.Stats.Application.Common.Interfaces;

namespace Twitter.Stats.API.Jobs
{
    internal class StreamTwitterProcessingJob : IHostedService, IDisposable
    {
        private Task? _executingTask;
        private readonly CancellationTokenSource _stoppingCancellationTokenSource = new();
        private readonly ILogger<StreamTwitterProcessingJob> _logger;
        private readonly IServiceProvider _serviceProvider;

        public StreamTwitterProcessingJob(
        IServiceProvider serviceProvider,
        ILogger<StreamTwitterProcessingJob> logger) =>
        (_serviceProvider, _logger) = (serviceProvider, logger);

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _executingTask = ExecuteAsync(_stoppingCancellationTokenSource.Token);

            if (_executingTask.IsCompleted)
            {
                return _executingTask;
            }

            return Task.CompletedTask;

        }


        public void Dispose()
        {
            _stoppingCancellationTokenSource.Cancel();
        }
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTask == null)
            {
                return;
            }

            _stoppingCancellationTokenSource.Cancel();

            await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
        }
        private async Task ExecuteAsync(CancellationToken cancellationToken)
        {

            try
            {
                _logger.LogInformation($"{nameof(StreamTwitterProcessingJob)} is running.");

                await DoWorkAsync(cancellationToken);
            }
            catch (Exception ex)
            {

                _logger.LogCritical("Error encountered: {ex}", ex);
#if DEBUG
                using (FileStream fs = new($"{nameof(StreamTwitterProcessingJob)}.error.log", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    StreamWriter write = new(fs);
                    write.BaseStream.Seek(0, SeekOrigin.End);
                    write.WriteLine($"{DateTime.Now}:{ex}");
                    write.WriteLine(Environment.NewLine);

                    write.Flush();
                    write.Close();
                    fs.Close();
                }
#endif
                await StopAsync(cancellationToken);
                //Restart on Failure
                await ExecuteAsync(cancellationToken);
            }
        }


        private async Task DoWorkAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                $"{nameof(StreamTwitterProcessingJob)} is working.");

            //Register StreamTwitterProcessingService as scoped service
            using IServiceScope scope = _serviceProvider.CreateScope();

            IStreamTwitterProcessingService streamTwitterProcessingService =
                scope.ServiceProvider.GetRequiredService<IStreamTwitterProcessingService>();

            await streamTwitterProcessingService.DoWorkAsync(cancellationToken);
        }

    }
}
