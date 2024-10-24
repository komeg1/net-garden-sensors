using System.Runtime.ConstrainedExecution;
using System.Text;
using Api;
using Newtonsoft.Json;
public static class Utils{
    private static JsonSerializer _s = new JsonSerializer();
    public static T Deserialize<T>(string json){
        return _s.Deserialize<T>(new JsonTextReader(new StringReader(json)))!;
    }   
    public static byte[] ExportToJson<T>(List<T> data, Guid userGuid)
    {
        return Export(JsonExporter<T>.Instance, data);
    }

    public static byte[] ExportToCsv<T>(List<T> data, Guid userGuid)
    {
        return Export(CsvExporter<T>.Instance, data);
    }

    private static byte[] Export<T>(IFileExporter<T> exporter, List<T> data)
    {
        return exporter.Export(data); 
    }
}