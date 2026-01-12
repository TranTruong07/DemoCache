using StackExchange.Redis;

namespace DemoForRedis.Redis
{
    public class RedisContext
    {
        public IDatabase Database { get; }
        private readonly ILogger<RedisContext> _logger;

        public RedisContext(IConnectionMultiplexer mux, ILogger<RedisContext> logger)
        {
            Database = mux.GetDatabase();
            _logger = logger;
        }

        public async Task<bool> IsConnected()
        {
            try
            {
                var result = await Database.PingAsync();
                if (result != TimeSpan.Zero)
                {
                    _logger.LogInformation("Redis connection successful.");
                    return true;
                }
                else
                {
                    _logger.LogError("Redis connection failed.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis connection error: {Message}", ex.Message);
                return false;
            }
        }
    }
}
