namespace Claims.Infrastructure.Queue
{
    public class QueuedHostedService : BackgroundService
    {
        private readonly IBackgroundQueue _queue;
        private readonly ILogger<QueuedHostedService> _logger;

        public QueuedHostedService(IBackgroundQueue queue, ILogger<QueuedHostedService> logger)
        {
            _queue = queue;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await _queue.DequeueAsync(stoppingToken);
                try
                {
                    await workItem(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing background work item");
                }
            }
        }
    }
}