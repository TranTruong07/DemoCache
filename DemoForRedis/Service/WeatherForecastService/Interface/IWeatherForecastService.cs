using DemoForRedis.Common.ResponseModel;
using DemoForRedis.Entites;
using DemoForRedis.Service.WeatherForecastService.Models.Input;

namespace DemoForRedis.Service.WeatherForecastService.Interface
{
    public interface IWeatherForecastService
    {
        Task<BaseResponse> CreateWeatherForecast(CreateWeatherForecastInput input);
        Task<Response<WeatherForecast>> GetWeatherForecastById(string id);
    }
}
