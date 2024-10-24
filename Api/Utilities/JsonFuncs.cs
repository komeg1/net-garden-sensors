using System.Runtime.ConstrainedExecution;
using Newtonsoft.Json;
public static class JsonFuncs{
    private static JsonSerializer _s = new JsonSerializer();
    public static T Deserialize<T>(string json){
        return _s.Deserialize<T>(new JsonTextReader(new StringReader(json)));
    }
}