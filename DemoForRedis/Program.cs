
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

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
            builder.Services.AddMemoryCache();

            builder.Services.AddScoped(typeof(MongoRepository<>));

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
