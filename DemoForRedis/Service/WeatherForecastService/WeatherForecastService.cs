using DemoForRedis.Common.ResponseModel;
using DemoForRedis.Database.WeatherForecast.Interface;
using DemoForRedis.Entites;
using DemoForRedis.Redis;
using DemoForRedis.Service.WeatherForecastService.Models.Input;

namespace DemoForRedis.Service.WeatherForecastService
{
    public class WeatherForecastService : Interface.IWeatherForecastService
    {
        private readonly IDAOWeatherForecast _dAOWeatherForecast;
        private readonly ILogger<WeatherForecastService> _logger;
        private readonly IWeatherForecastCache _weatherForecastCache;

        public WeatherForecastService(IDAOWeatherForecast dAOWeatherForecast, ILogger<WeatherForecastService> logger, IWeatherForecastCache weatherForecastCache)
        {
            _dAOWeatherForecast = dAOWeatherForecast;
            _logger = logger;
            _weatherForecastCache = weatherForecastCache;
        }

        /// <summary>
        /// Tạo mới WeatherForecast
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<BaseResponse> CreateWeatherForecast(CreateWeatherForecastInput input)
        {
            try
            {
                // validate input
                if (input == null)
                {
                    return new BaseResponse
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Invalid input"
                    };
                }
                // các validate khác nếu có
                var weatherForecast = new WeatherForecast
                {
                    Date = input.Date,
                    TemperatureC = input.TemperatureC,
                    Summary = input.Summary
                };

                var result = await _dAOWeatherForecast.AddAsync(weatherForecast);
                if(!result)
                {
                    _logger.LogError("Failed to create weather forecast in database");
                    return new BaseResponse
                    {
                        StatusCode = StatusCodes.Status500InternalServerError,
                        Message = "Failed to create weather forecast"
                    };
                }

                // Cache vào Redis
                var cacheKey = $"weatherforecast-{weatherForecast.Id}";
                var cacheValue = System.Text.Json.JsonSerializer.Serialize(weatherForecast);
                var cacheResult = await _weatherForecastCache.SetWeatherForecastAsync(cacheKey, cacheValue, TimeSpan.FromHours(1));
                if (!cacheResult)
                {
                    _logger.LogWarning("Failed to cache weather forecast in Redis");
                }

                return new BaseResponse
                {
                    StatusCode = StatusCodes.Status201Created,
                    Message = "Weather forecast created successfully"
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in service creating weather forecast: {ex.Message}");
                return new BaseResponse
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Error creating weather forecast"
                };
            }
        }

        /// <summary>
        /// Lấy WeatherForecast theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Response<WeatherForecast>> GetWeatherForecastById(string id)
        {
            try
            {
                // validate input
                if (string.IsNullOrEmpty(id))
                {
                    return new Response<WeatherForecast>
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Invalid Id"
                    };
                }
                // Kiểm tra cache trước
                var cacheKey = $"weatherforecast-{id}";
                var cachedData = await _weatherForecastCache.GetWeatherForecastAsync(cacheKey);
                if (!string.IsNullOrEmpty(cachedData))
                {
                    var weatherForecastFromCache = System.Text.Json.JsonSerializer.Deserialize<WeatherForecast>(cachedData);
                    return new Response<WeatherForecast>
                    {
                        StatusCode = StatusCodes.Status200OK,
                        Message = "Weather forecast retrieved successfully from cache",
                        Data = weatherForecastFromCache
                    };
                }

                // Nếu không có trong cache, lấy từ database
                var weatherForecast = await _dAOWeatherForecast.GetByIdAsync(id);
                if (weatherForecast == null)
                {
                    return new Response<WeatherForecast>
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Weather forecast not found"
                    };
                }
                return new Response<WeatherForecast>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Weather forecast retrieved successfully",
                    Data = weatherForecast
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in service retrieving weather forecast by Id: {ex.Message}");
                return new Response<WeatherForecast>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Error retrieving weather forecast"
                };
            }
        }
    }
}
