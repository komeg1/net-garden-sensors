namespace Api;

public class SensorsDatabaseSettings{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string SensorsCollectionName { get; set; } = null!;
}