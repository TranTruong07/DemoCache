
namespace DemoForRedis.Database
{
    public class MongdbWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<MongdbWorker> _logger;
        public MongdbWorker(IServiceScopeFactory serviceScopeFactory, ILogger<MongdbWorker> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var scope = _serviceScopeFactory.CreateScope();
                var mongodbContext = scope.ServiceProvider.GetRequiredService<MongodbContext>();

                var isConnected = await mongodbContext.IsConnected();
                if (isConnected)
                {
                    _logger.LogInformation("MongdbWorker started and connected to MongoDB.");
                }
                else
                {
                    _logger.LogError("MongdbWorker failed to connect to MongoDB.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MongdbWorker error: {Message}", ex.Message);
            }
        }
    }
}
