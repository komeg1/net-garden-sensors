namespace Api;

public class MqttSettings{
    public required string Address {get;set;}
    public required int Port {get;set;}
    public required string ClientId{get;set;}
    public required string Topic {get;set;}

    public override string ToString(){
        return $"{Address}:{Port}, clientId: {ClientId}, topic: {Topic}";
    }  
}