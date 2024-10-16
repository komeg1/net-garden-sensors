
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
namespace Api;

public class SensorDataService : ISensorDataService
{
    private readonly IMongoCollection<SensorData> _sensorDataCollection;

    public SensorDataService(
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
            await _sensorDataCollection.InsertOneAsync(newSensorData);
            Console.WriteLine("dodane");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        
        
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

   public async Task<List<SensorData>> GetNewestDataAsync() {
    var pipeline = new BsonDocument[]
        {
            new BsonDocument("$sort", new BsonDocument("Timestamp", -1)),  // Sort by Timestamp in descending order
            new BsonDocument("$group", new BsonDocument
            {
                { "_id", "$SensorId" },  // Group by SensorId
                { "latestRecord", new BsonDocument("$first", "$$ROOT") }  // Get the first (latest) record for each group
            }),
            new BsonDocument("$replaceRoot", new BsonDocument("newRoot", "$latestRecord"))  // Replace the root with the latest record
        };
    
    var result = await _sensorDataCollection.Aggregate<SensorData>(pipeline).ToListAsync();
    return result;
    }
}