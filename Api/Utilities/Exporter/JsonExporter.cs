using Api;
using Newtonsoft.Json;

public class JsonExporter<T> : FileExporter<T>
{
    public JsonExporter(string filesPath, Guid userGuid) : base(filesPath, userGuid) { }

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