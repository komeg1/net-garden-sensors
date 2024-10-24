using System.Text;
using Api;

public class CsvExporter<T> : FileExporter<T>
{
    public CsvExporter(string filesPath, Guid userGuid) : base(filesPath, userGuid) { }

    public override byte[] Export(List<T> data)
    {
        try
        {
            var sb = new StringBuilder();
            var filePath = GetFilePath(ExportFormat.CSV);
            var header = string.Join("; ", typeof(T).GetProperties().Select(p => p.Name));
            sb.AppendLine(header);

            foreach (var item in data)
            {
                var line = string.Join("; ", typeof(T).GetProperties().Select(p => p.GetValue(item, null)?.ToString() ?? string.Empty));
                sb.AppendLine(line);
            }

            File.WriteAllText(filePath, sb.ToString());
            LogSuccess(ExportFormat.CSV);
            return File.ReadAllBytes(filePath);
        }
        catch (Exception ex)
        {     
            Logger.Instance.Log(this, new LogEventArgs(ex.Message, Api.LogLevel.Error));
            return new byte[0];
        }
    }

        
}
