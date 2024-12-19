using Api;

public abstract class FileExporter<T> : IFileExporter<T>
{
    protected static readonly string _FILES_PATH = Path.Combine(Directory.GetCurrentDirectory(), "GeneratedFiles");
    public FileExporter()
    {
        CreateFilesDirectory();
    }

    public abstract byte[] Export(List<T> data);

    public void CreateFilesDirectory()
    {
        if (!Directory.Exists(_FILES_PATH))
        {
            Directory.CreateDirectory(_FILES_PATH);
        }
    }

    public string GetFilePath(ExportFormat format) =>
        Path.Combine(_FILES_PATH, $"{Guid.NewGuid}.{(format == ExportFormat.CSV ? "csv" : "json")}");

    public void LogSuccess(ExportFormat format){
        Logger.Instance.Log(this, new LogEventArgs($"Exported {format} successfully", Api.LogLevel.Success));
    }



}