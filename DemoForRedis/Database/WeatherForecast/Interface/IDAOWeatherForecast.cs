namespace DemoForRedis.Database.WeatherForecast.Interface
{
    public interface IDAOWeatherForecast
    {
        Task<bool> AddAsync(Entites.WeatherForecast model);
        Task<Entites.WeatherForecast?> GetByIdAsync(string id);
    }
}
