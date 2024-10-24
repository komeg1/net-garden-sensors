using System.Runtime.ConstrainedExecution;
using System.Text;
using Api;
using Newtonsoft.Json;
public static class Utils{
    private static JsonSerializer _s = new JsonSerializer();
    private static readonly string _FILES_PATH = Path.Combine(Directory.GetCurrentDirectory(), "GeneratedFiles");
    public static T Deserialize<T>(string json){
        return _s.Deserialize<T>(new JsonTextReader(new StringReader(json)));
    }   
    public static byte[] ExportToJson<T>(List<T> data, Guid userGuid)
    {
        return Export(new JsonExporter<T>(_FILES_PATH, userGuid), data);
    }

    public static byte[] ExportToCsv<T>(List<T> data, Guid userGuid)
    {
        return Export(new CsvExporter<T>(_FILES_PATH, userGuid), data);
    }

    private static byte[] Export<T>(IFileExporter<T> exporter, List<T> data)
    {
        return exporter.Export(data); 
    }
}