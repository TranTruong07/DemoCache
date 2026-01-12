using MongoDB.Driver;

namespace DemoForRedis.Database.WeatherForecast
{
    public class DAOWeatherForecast : Interface.IDAOWeatherForecast
    {
        private readonly MongodbContext _context;
        private readonly ILogger<DAOWeatherForecast> _logger;
        private readonly IMongoCollection<Entites.WeatherForecast> _collection;
        public DAOWeatherForecast(MongodbContext context, ILogger<DAOWeatherForecast> logger)
        {
            _context = context;
            _logger = logger;
            _collection = _context.mongoDatabase.GetCollection<Entites.WeatherForecast>(typeof(Entites.WeatherForecast).Name);
        }

        public async Task<bool> AddAsync(Entites.WeatherForecast model)
        {
            try
            {
                await _collection.InsertOneAsync(model);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding WeatherForecast: {Message}", ex.Message);
                return false;
            }
        }

        public async Task<Entites.WeatherForecast?> GetByIdAsync(string id)
        {
            try
            {
                return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving WeatherForecast by Id: {Message}", ex.Message);
                return null;
            }
        }
    }
}
