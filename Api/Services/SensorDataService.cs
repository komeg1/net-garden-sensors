
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
namespace Api;

public class SensorDataSevice : ISensorDataService
{
    private readonly IMongoCollection<SensorData> _sensorDataCollection;
    

    public SensorDataSevice(
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

    public async Task<List<SensorData>> GetAsync(FilterDefinition<SensorData> filter)
    {
       return await _sensorDataCollection.Find(filter).ToListAsync();
    }

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

    public async Task<byte[]> ExportToFile(ExportFormat format){
        var data = await GetAsync();

        if (data == null)
            return null;

        if (format == ExportFormat.JSON)
            return Utils.ExportToJson(data, Guid.NewGuid());

        if (format == ExportFormat.CSV)
            return Utils.ExportToCsv(data, Guid.NewGuid());

        return null;

    }

    
}

