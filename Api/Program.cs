namespace Api;
using System.Text.Json;
using Api.Entities.Config;
using Microsoft.Net.Http.Headers;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin",
                builder => builder.WithOrigins("http://127.0.0.1:5500") 
                                .AllowAnyHeader()
                                .AllowAnyMethod());
        });

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.Configure<SensorsDatabaseSettings>(
            builder.Configuration.GetSection("Database"));
        builder.Services.Configure<BlockchainSettings>(
            builder.Configuration.GetSection("Blockchain"));
            
        builder.Services.Configure<MqttSettings>(builder.Configuration.GetSection("MqttSettings"));

        builder.Services.AddSingleton<ISensorDataService,SensorDataService>();
        builder.Services.AddSingleton<MqttService>();

        builder.Services.AddSingleton<IBlockchainService,BlockchainService>();
        builder.Services.AddSingleton<IWalletService,WalletService>(provider =>
        {
            var mnemonic = "circle vital cake know boat crumble entry rubber sleep beach anchor mercy";
            return new WalletService(mnemonic);
        });

        var app = builder.Build();
        app.UseCors("AllowSpecificOrigin");
        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI();


        app.UseHttpsRedirection();

        app.UseAuthorization(); 
        var sensorService = app.Services.GetRequiredService<ISensorDataService>();
        app.MapGet("/sse", async (HttpContext ctx) =>
        {
            ctx.Response.Headers.Append(HeaderNames.ContentType, "text/event-stream");
            while (!ctx.RequestAborted.IsCancellationRequested)
            {
                if(sensorService.DataQueue.TryTake(out var data))
                {
                await ctx.Response.WriteAsync($"data: ");
                await JsonSerializer.SerializeAsync(ctx.Response.Body, data);
                await ctx.Response.WriteAsync($"\n\n");
                await ctx.Response.Body.FlushAsync();
                Logger.Instance.Log(nameof(ctx),message:new LogEventArgs(message:"Send SSE event",LogLevel.Debug));
                }
            };
        });

        app.MapControllers();

        //start mqtt
        var mqttService = app.Services.GetRequiredService<MqttService>();
        Task.Run(() =>mqttService.Connect()).Wait();


        
        app.Run();

        
    }
}
