using System.Text;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;

namespace Api;

public class MqttService : IMqttService
{
    private readonly IMqttClient _mqttClient;
    private readonly MqttSettings _mqttSettings;

    public MqttService(IOptions<MqttSettings> mqttSettings){
        _mqttClient = new MqttFactory()
                            .CreateMqttClient();    

        ValidateMqttSettings(mqttSettings.Value);
        _mqttSettings = mqttSettings.Value;

    }
    
    public async Task Connect()
    {

          var options = new MqttClientOptionsBuilder()
            .WithClientId(_mqttSettings.ClientId)
            .WithTcpServer(_mqttSettings.Address,_mqttSettings.Port)
            .Build();

        await _mqttClient.ConnectAsync(options);
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
        Console.WriteLine($"Received message: {payload} from topic: {e.ApplicationMessage.Topic}");
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