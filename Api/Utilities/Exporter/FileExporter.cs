using Api;

public abstract class FileExporter<T> : IFileExporter<T>
{
    protected string _filesPath;
    protected Guid _userGuid;

    public FileExporter(string filesPath, Guid userGuid)
    {
        _filesPath = filesPath;
        _userGuid = userGuid;
        CreateFilesDirectory();
    }

    public abstract byte[] Export(List<T> data);

    protected void CreateFilesDirectory()
    {
        if (!Directory.Exists(_filesPath))
        {
            Directory.CreateDirectory(_filesPath);
        }
    }

    protected string GetFilePath(ExportFormat format) =>
        Path.Combine(_filesPath, $"{_userGuid}.{(format == ExportFormat.CSV ? "csv" : "json")}");

    protected void LogSuccess(ExportFormat format){
        Logger.Instance.Log(this, new LogEventArgs($"Exported {format} successfully", Api.LogLevel.Success));
    }
}