namespace DemoForRedis.Redis
{
    public class RedisWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<RedisWorker> _logger;

        public RedisWorker(IServiceScopeFactory serviceScopeFactory, ILogger<RedisWorker> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var scope = _serviceScopeFactory.CreateScope();
                var redisContext = scope.ServiceProvider.GetRequiredService<RedisContext>();
                var isConnected = await redisContext.IsConnected();
                if (isConnected)
                {
                    _logger.LogInformation("RedisWorker started and connected to Redis.");
                }
                else
                {
                    _logger.LogError("RedisWorker failed to connect to Redis.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RedisWorker error: {Message}", ex.Message);
            }
        }
    }
}
