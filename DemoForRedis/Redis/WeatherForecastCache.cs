namespace DemoForRedis.Redis
{
    public class WeatherForecastCache : IWeatherForecastCache
    {
        private readonly RedisContext _redisContext;
        private readonly ILogger<WeatherForecastCache> _logger;

        public WeatherForecastCache(RedisContext redisContext, ILogger<WeatherForecastCache> logger)
        {
            _redisContext = redisContext;
            _logger = logger;
        }

        public async Task<bool> SetWeatherForecastAsync(string key, string value, TimeSpan? expiry = null)
        {
            try
            {
                return await _redisContext.Database.StringSetAsync(key, value, expiry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting WeatherForecast in Redis: {Message}", ex.Message);
                return false;
            }
        }

        public async Task<string?> GetWeatherForecastAsync(string key)
        {
            try
            {
                return await _redisContext.Database.StringGetAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting WeatherForecast from Redis: {Message}", ex.Message);
                return null;
            }
        }
    }
}
