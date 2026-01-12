using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DemoForRedis.Entites
{
    [BsonIgnoreExtraElements]
    public class WeatherForecast
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public DateOnly Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }
    }
}
