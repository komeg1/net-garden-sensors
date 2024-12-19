using Api;

public interface IFileExporter<T>
{
    byte[] Export(List<T> data);
    void CreateFilesDirectory();
    string GetFilePath(ExportFormat format);
    void LogSuccess(ExportFormat format);
}