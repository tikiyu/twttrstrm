using Microsoft.Extensions.Options;
using NCrontab;
using Twitter.Stats.Application.Common.Interfaces;
using Twitter.Stats.Application.Common.Settings;

namespace Twitter.Stats.API.Jobs
{
    internal class DayTrendingHashTagCronJob : IHostedService, IDisposable
    {
        private Task? _executingTask;
        private readonly CancellationTokenSource _stoppingCancellationTokenSource = new();
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DayTrendingHashTagCronJob> _logger;
        private readonly HashtagSettings _hashtagSettings;
        private readonly CrontabSchedule _schedule;
        private Timer _timer;
        private DateTime _nextRun;

        public DayTrendingHashTagCronJob(
        IServiceProvider serviceProvider,
        ILogger<DayTrendingHashTagCronJob> logger,
        IOptions<HashtagSettings> hashtagSettings)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _hashtagSettings = hashtagSettings.Value;
            _schedule = CrontabSchedule.Parse(_hashtagSettings.DayTrendingCronJobSchedule);
            _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _executingTask = ExecuteAsync(_stoppingCancellationTokenSource.Token);

            if (_executingTask.IsCompleted)
            {
                return _executingTask;
            }

            return Task.CompletedTask;
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
                _logger.LogInformation($"{nameof(DayTrendingHashTagCronJob)} has started.");

                await DoWorkAsync(cancellationToken);

            }
            catch (Exception ex)
            {

                _logger.LogCritical("Error encountered: {ex}", ex);

#if DEBUG 
                using (FileStream fs = new($"{nameof(DayTrendingHashTagCronJob)}.error.log", FileMode.OpenOrCreate, FileAccess.Write))
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
                $"{nameof(StreamTweetsJob)} is working.");


            while (!cancellationToken.IsCancellationRequested)
            {
                // Check if it's time to run the job based on the cron expression
                var now = DateTime.Now;

                if (now > _nextRun)
                {
                    // Run the job
                    _logger.LogInformation($"Running {nameof(DayTrendingHashTagCronJob)} at {DateTime.Now}");

                    //Register DayTrendingHashTagService as scoped service
                    using IServiceScope scope = _serviceProvider.CreateScope();

                    IDayTrendingHashTagService dayTrendingHashTagService =
                        scope.ServiceProvider.GetRequiredService<IDayTrendingHashTagService>();

                    await dayTrendingHashTagService.DoWorkAsync(cancellationToken);

                    _nextRun = _schedule.GetNextOccurrence(now);
                }

                var delay = _nextRun - now;

                await Task.Delay(delay, cancellationToken);
            }
        }

        public void Dispose()
        {
            _stoppingCancellationTokenSource.Cancel();
        }
    }

}
