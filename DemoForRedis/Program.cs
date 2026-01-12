
using DemoForRedis.Config;
using DemoForRedis.Database;
using DemoForRedis.Database.WeatherForecast;
using DemoForRedis.Database.WeatherForecast.Interface;
using DemoForRedis.Service.WeatherForecastService;
using DemoForRedis.Service.WeatherForecastService.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using StackExchange.Redis;

namespace DemoForRedis
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // add log file
            builder.Logging.AddFile(pathFormat: "Logs/log-{Date}.txt",
                fileSizeLimitBytes: null,
                retainedFileCountLimit: 20,
                isJson: false);

            builder.Services.Configure<MongoConfig>(builder.Configuration.GetSection("MongoConfig"));
            var mongoConfig = builder.Configuration.GetSection("MongoConfig").Get<MongoConfig>();
            if (string.IsNullOrEmpty(mongoConfig?.ConnectionString) || string.IsNullOrEmpty(mongoConfig.DatabaseName))
            {
                throw new ArgumentException("MongoDB configuration is not properly set in appsettings.json");
            }
            builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
            {
                var settings = MongoClientSettings.FromConnectionString(mongoConfig.ConnectionString);
                settings.ConnectTimeout = TimeSpan.FromSeconds(30);
                settings.SocketTimeout = TimeSpan.FromSeconds(30);
                settings.RetryReads = true;
                settings.RetryWrites = true;
                return new MongoClient(settings);
            });

            builder.Services.AddScoped<IMongoDatabase>(serviceProvider =>
            {
                var client = serviceProvider.GetRequiredService<IMongoClient>();
                return client.GetDatabase(mongoConfig.DatabaseName);
            });
            // setup MongoDB 
            var conventionPack = new ConventionPack
            {
                new IgnoreExtraElementsConvention(true)
            };
            ConventionRegistry.Register("IgnoreExtraElements", conventionPack, t => true);

            // cache 
            string redisConnectionString = builder.Configuration["RedisConfig:ConnectionString"] ?? throw new Exception("Redis connection string is not configured.");
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "DemoForRedisInstance";
            });
            // set up the connection multiplexer
            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                return ConnectionMultiplexer.Connect(redisConnectionString);
            });
            
            builder.Services.AddMemoryCache();

            // DI DAO
            builder.Services.AddScoped<MongodbContext>();
            builder.Services.AddScoped<IDAOWeatherForecast, DAOWeatherForecast>();

            // DI Services
            builder.Services.AddScoped<IWeatherForecastService, WeatherForecastService>();


            // DI Redis
            builder.Services.AddScoped<Redis.RedisContext>();
            builder.Services.AddScoped<Redis.IWeatherForecastCache, Redis.WeatherForecastCache>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
