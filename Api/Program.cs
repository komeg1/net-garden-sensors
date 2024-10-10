
namespace Api;

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
        builder.Services.Configure<SensorsDatabaseSettings>(
            builder.Configuration.GetSection("Database"));
            
        builder.Services.Configure<MqttSettings>(builder.Configuration.GetSection("MqttSettings"));

        builder.Services.AddScoped<ISensorsService,SensorsService>();
        //builder.Services.AddSingleton<MqttService>();


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

        //start mqtt
        
        //var mqttService = app.Services.GetRequiredService<MqttService>();
        //Task.Run(() =>mqttService.Connect()).Wait();
        

        app.Run();

        
    }
}
