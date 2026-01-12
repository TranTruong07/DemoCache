using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace DemoForRedis.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        private readonly MongoRepository<WeatherForecast> _repository;

        public WeatherForecastController(IMemoryCache memoryCache, MongoRepository<WeatherForecast> repository)
        {
            _memoryCache = memoryCache;
            _repository = repository;
        }

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IActionResult> Get([FromQuery] string id)
        {
            var weatherForecast = await _repository.GetByIdAsync(id);
            if (weatherForecast == null)
            {
                return NotFound();
            }

            return Ok(weatherForecast);
        }

        [HttpPost("create-cache")]
        public async Task<IActionResult> SetCache([FromBody] CacheRequest request)
        {
            var key = $"weatherforecast-{request.Date}";
            var weatherForecast = new WeatherForecast
            {
                Date = request.Date,
                TemperatureC = request.TemperatureC,
                Summary = request.Summary
            };

            await _repository.AddAsync(weatherForecast);
            // Store in memory cache for 5 minutes

            _memoryCache.Set(key, weatherForecast, TimeSpan.FromMinutes(5));
            return Ok(weatherForecast);
        }

        [HttpGet("get-cache")]
        public IActionResult GetCache([FromQuery] DateOnly date)
        {
            var key = $"weatherforecast-{date}";
            if (_memoryCache.TryGetValue<WeatherForecast>(key, out var weatherForecast))
            {
                return Ok(weatherForecast);
            }
            return NotFound();
        }

    }

    public class CacheRequest
    {
        public DateOnly Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }
    }
}
