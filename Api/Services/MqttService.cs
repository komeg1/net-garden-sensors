using System.Text;
using Microsoft.Extensions.Options;
using MongoDB.Bson.IO;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Api;

public class MqttService : IMqttService
{
    private readonly IMqttClient _mqttClient;
    private readonly IServiceProvider _serviceProvider;
    private readonly MqttSettings _mqttSettings;
    public MqttService(IOptions<MqttSettings> mqttSettings, IServiceProvider serviceProvider){
        _mqttClient = new MqttFactory()
                            .CreateMqttClient();    

        ValidateMqttSettings(mqttSettings.Value);
        _mqttSettings = mqttSettings.Value;
        _serviceProvider = serviceProvider;
       

    }
    
    public async Task Connect()
    {

          var options = new MqttClientOptionsBuilder()
            .WithClientId(_mqttSettings.ClientId)
            .WithTcpServer(_mqttSettings.Address,_mqttSettings.Port)
            .Build();
        
         while(true)
         {
            try
            {
                await _mqttClient.ConnectAsync(options);
                Console.WriteLine("connected");
                break;
            }
            catch (Exception ex)
            {
                    Console.WriteLine($"Connection failed: {ex.Message}. Retrying in {5000 / 1000} seconds...");
                    await Task.Delay(5000);
            }
        }

         
        
        await _mqttClient.SubscribeAsync(_mqttSettings.Topic);
        Console.WriteLine($"connected to {_mqttSettings}");
        _mqttClient.ApplicationMessageReceivedAsync += HandleReceivedMessage;
        
    }

    public async Task DisconnectAsync()
    {
        await _mqttClient.DisconnectAsync();
    }

    public async Task HandleReceivedMessage(MqttApplicationMessageReceivedEventArgs e)
    {
        var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
        var deserializedPayload = JObject.Parse(payload);
        Console.WriteLine($"Received message: {deserializedPayload}");
        using (var scope = _serviceProvider.CreateScope())
        {
            var ctx = scope.ServiceProvider.GetRequiredService<ISensorDataService>();

            var guid = BitConverter.ToString(Guid.NewGuid().ToByteArray())
            .Replace("-", "")
            .ToLower()
            .Substring(0,24);

        await ctx.CreateAsync(new SensorData{Id=guid,
                                                   SensorId = deserializedPayload["sensorid"].Value<int>(),
                                                   Value= deserializedPayload["temperature"].Value<float>(),
                                                   Unit=deserializedPayload["unit"].ToString(),
                                                   Timestamp=DateTime.Parse(deserializedPayload["timestamp"].ToString())});
        }

        
        

    }

    public void ValidateMqttSettings(MqttSettings mqttSettings){
        if(string.IsNullOrWhiteSpace(mqttSettings.ClientId))
            throw new ArgumentException("MqttSettings ClientID can't be null/blank");
        if(string.IsNullOrWhiteSpace(mqttSettings.Address))
            throw new ArgumentException("MqttSettings Address can't be null/blank");
        if(string.IsNullOrWhiteSpace(mqttSettings.Port.ToString()))
            throw new ArgumentException("MqttSettings Port can't be null/blank");
        if(string.IsNullOrWhiteSpace(mqttSettings.Topic))
            throw new ArgumentException("MqttSettings Topic can't be null/blank");
        
    }
}