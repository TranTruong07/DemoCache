using DemoForRedis.Entites;
using DemoForRedis.Service.WeatherForecastService.Interface;
using DemoForRedis.Service.WeatherForecastService.Models.Input;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;

namespace DemoForRedis.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IWeatherForecastService _weatherForecastService;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(IMemoryCache memoryCache, IWeatherForecastService weatherForecastService, ILogger<WeatherForecastController> logger)
        {
            _memoryCache = memoryCache;
            _weatherForecastService = weatherForecastService;
            _logger = logger;
        }

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };


        /// <summary>
        /// Tạo mới WeatherForecast
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("create-weather-forecast")]
        public async Task<IActionResult> SetCache([FromBody] CreateWeatherForecastInput request)
        {
            try
            {
                var response = await _weatherForecastService.CreateWeatherForecast(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in controller creating weather forecast: {Message}", ex.Message);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy WeatherForecast theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("get-weather-forecast")]
        public async Task<IActionResult> GetCache([FromQuery] string id)
        {
            try
            {
                var res = await _weatherForecastService.GetWeatherForecastById(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in controller getting weather forecast: {Message}", ex.Message);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }


}
