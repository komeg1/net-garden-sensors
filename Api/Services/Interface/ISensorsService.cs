
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Api;

public interface ISensorsService
{
   

    Task<List<SensorData>> GetAsync();
    Task<SensorData?> GetAsync(string id);
    Task CreateAsync(SensorData newSensorData);
    Task UpdateAsync(string id, SensorData updatedSensorData);
    Task RemoveAsync(string id);
}