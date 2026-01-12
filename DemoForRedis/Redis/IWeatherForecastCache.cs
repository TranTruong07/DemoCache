namespace DemoForRedis.Redis
{
    public interface IWeatherForecastCache
    {
        Task<bool> SetWeatherForecastAsync(string key, string value, TimeSpan? expiry = null);
        Task<string?> GetWeatherForecastAsync(string key);
    }
}
