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
        CreateFilesDirectory();

        var json = JsonConvert.SerializeObject(data);
        string filePath = Path.Combine(_FILES_PATH, $"{userGuid}.json");

         File.WriteAllText(filePath, json);
         return File.ReadAllBytes(filePath);

    }


    public static byte[] ExportToCsv<T>(List<T> data, Guid userGuid)
    {
        CreateFilesDirectory();
        var sb = new StringBuilder();
        string filePath = GetFilePath(ExportFormat.CSV, userGuid);
        var header = "";
        var entityInfo = typeof(T).GetProperties();
        
        foreach (var prop in entityInfo)
            header += prop.Name + "; ";
        

        header = header.Substring(0, header.Length - 2);
        sb.AppendLine(header);

        TextWriter sw = new StreamWriter(filePath, true);
        sw.Write(sb.ToString());
       

        foreach (var sensor in data)
        {
            sb = new StringBuilder();
            var line ="";
            foreach(var prop in entityInfo)
                line += prop.GetValue(sensor, null) + "; ";
            line = line.Substring(0, line.Length - 2);
            sb.AppendLine(line);
            sw.Write(sb.ToString());

        }
         sw.Close();

        return File.ReadAllBytes(filePath);

    }

    
    private static void CreateFilesDirectory()
    {
        if(!Directory.Exists(_FILES_PATH))
        {
            Directory.CreateDirectory(_FILES_PATH);
        }   
    }

    private static string GetFilePath(ExportFormat format, Guid userGuid) => Path.Combine(_FILES_PATH, $"{userGuid}.{(format == ExportFormat.CSV ? "csv" : "json")}");

}