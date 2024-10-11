
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Api;

public class SensorsService : ISensorsService
{
    private readonly IMongoCollection<SensorData> _sensorDataCollection;

    public SensorsService(
        IOptions<SensorsDatabaseSettings> sensorsDatabaseSettings)
    {
        Console.WriteLine("connecting to db");
        var mongoClient = new MongoClient(
            sensorsDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            sensorsDatabaseSettings.Value.DatabaseName);

        Console.WriteLine(sensorsDatabaseSettings.Value.SensorsCollectionName);
        _sensorDataCollection = mongoDatabase.GetCollection<SensorData>(
            sensorsDatabaseSettings.Value.SensorsCollectionName);
        Console.WriteLine("connected to db");
    }

    public async Task<List<SensorData>> GetAsync() =>
        await _sensorDataCollection.Find(_ => true).ToListAsync();

    public async Task<SensorData?> GetAsync(string id) =>
        await _sensorDataCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(SensorData newSensorData) {
        try
        {
        Console.WriteLine(newSensorData.Timestamp.ToString());
         _sensorDataCollection.InsertOne(newSensorData);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        Console.WriteLine("dodane");
        var filter = Builders<SensorData>.Filter
                .Eq(r => r.Unit, "C");

            // Finds the newly inserted document by using the filter
        var document = _sensorDataCollection.Find(filter).FirstOrDefault();
        Console.WriteLine("", document);
    }
        

    public async Task UpdateAsync(string id, SensorData updatedSensorData) =>
        await _sensorDataCollection.ReplaceOneAsync(x => x.Id == id, updatedSensorData);

    public async Task RemoveAsync(string id) =>
        await _sensorDataCollection.DeleteOneAsync(x => x.Id == id);
}