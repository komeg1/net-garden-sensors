using Api;
using Newtonsoft.Json;

public class JsonExporter<T> : FileExporter<T>
{
    private static JsonExporter<T> _instance;
    private JsonExporter() {}

    public static JsonExporter<T> Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new JsonExporter<T>();
            }
            return _instance;
        }
    }

    public override byte[] Export(List<T> data)
    {
        try
        {
            var json = JsonConvert.SerializeObject(data);
            var filePath = GetFilePath(ExportFormat.JSON);
            File.WriteAllText(filePath, json);
            LogSuccess(ExportFormat.JSON);
            return File.ReadAllBytes(filePath);
        }
        catch (Exception ex)
        {
            Logger.Instance.Log(this, new LogEventArgs(ex.Message, Api.LogLevel.Error));
            return new byte[0];
        }
    }
}