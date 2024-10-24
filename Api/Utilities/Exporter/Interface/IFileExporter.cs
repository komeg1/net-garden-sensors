public interface IFileExporter<T>
{
    byte[] Export(List<T> data);
}