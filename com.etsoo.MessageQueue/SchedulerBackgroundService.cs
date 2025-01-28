using Cronos;
using Microsoft.Extensions.Hosting;

namespace com.etsoo.MessageQueue
{
    /// <summary>
    /// Scheduler background service with CRON
    /// 使用 CRON 的调度器后台服务
    /// </summary>
    public abstract class SchedulerBackgroundService : BackgroundService
    {
        private readonly CronExpression _cron;
        private readonly TimeZoneInfo _timeZone;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="cron">CRON expression</param>
        /// <param name="timeZone">Time zone, default is Local</param>
        public SchedulerBackgroundService(string cron, TimeZoneInfo? timeZone = null)
        {
            _cron = CronExpression.Parse(cron);
            _timeZone = timeZone ?? TimeZoneInfo.Local;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ScheduleJobAsync(stoppingToken);
                await ExecuteJobAsync(stoppingToken);
            }
        }

        /// <summary>
        /// Execute job async
        /// 异步执行作业
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        abstract protected Task ExecuteJobAsync(CancellationToken cancellationToken);

        private async Task ScheduleJobAsync(CancellationToken cancellationToken)
        {
            var next = _cron.GetNextOccurrence(DateTimeOffset.Now, _timeZone) ?? throw new InvalidOperationException("Invalid CRON expression");
            var delay = next - DateTimeOffset.Now;
            await Task.Delay(delay, cancellationToken);
        }
    }
}
