
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Api;

public interface ISensorDataService
{
   

    Task<List<SensorData>> GetAsync();
    Task<SensorData?> GetAsync(string id);
    Task<List<SensorData>> GetAsync(FilterDefinition<SensorData> filter);
    Task CreateAsync(SensorData newSensorData);
    Task UpdateAsync(string id, SensorData updatedSensorData);
    Task RemoveAsync(string id);
    Task<List<SensorData>> GetNewestDataAsync();
    Task<byte[]> ExportToFile(ExportFormat format);

}