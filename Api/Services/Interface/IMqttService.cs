namespace Api;
using MQTTnet;
using MQTTnet.Client;
public interface IMqttService{
    Task Connect();
    Task HandleReceivedMessage(MqttApplicationMessageReceivedEventArgs e);
    Task DisconnectAsync();
}