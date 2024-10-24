
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
namespace Api;

public class SensorDataSevice : ISensorDataService
{
    private readonly IMongoCollection<SensorData> _sensorDataCollection;
    private event EventHandler<LogEventArgs> OnLog;

    public SensorDataSevice(IOptions<SensorsDatabaseSettings> sensorsDatabaseSettings)
    {
    
        OnLog += Logger.Instance.Log;
        OnLog?.Invoke(this,new LogEventArgs("connecting to db", LogLevel.Warning));
        var mongoClient = new MongoClient(
            sensorsDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            sensorsDatabaseSettings.Value.DatabaseName);

        OnLog?.Invoke(this,new LogEventArgs($"Collection: {sensorsDatabaseSettings.Value.SensorsCollectionName}",LogLevel.Debug));
        _sensorDataCollection = mongoDatabase.GetCollection<SensorData>(
            sensorsDatabaseSettings.Value.SensorsCollectionName);
        OnLog?.Invoke(this,new LogEventArgs("connected to db", LogLevel.Success));
    }

    public async Task<List<SensorData>> GetAsync() =>
        await _sensorDataCollection.Find(_ => true).ToListAsync();

    public async Task<SensorData?> GetAsync(string id) =>
        await _sensorDataCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<List<SensorData>> GetAsync(PipelineDefinition<SensorData,SensorData>? pipeline)
    {
       return await _sensorDataCollection.Aggregate(pipeline).ToListAsync();
    }

    public async Task CreateAsync(SensorData newSensorData) {
        try
        {
        OnLog?.Invoke(this,new LogEventArgs(newSensorData.Timestamp.ToString()));
         _sensorDataCollection.InsertOne(newSensorData);
        }
        catch (Exception e)
        {
            OnLog?.Invoke(this,new LogEventArgs(e.Message,LogLevel.Error));
        }
        
    }
        
    public async Task UpdateAsync(string id, SensorData updatedSensorData) =>
        await _sensorDataCollection.ReplaceOneAsync(x => x.Id == id, updatedSensorData);

    public async Task RemoveAsync(string id) =>
        await _sensorDataCollection.DeleteOneAsync(x => x.Id == id);

   public async Task<List<SensorData>> GetNewestDataAsync() {
    var pipeline = new BsonDocument[]
        {
            new BsonDocument("$sort", new BsonDocument("Timestamp", -1)), 
            new BsonDocument("$group", new BsonDocument
            {
                { "_id", "$SensorId" },  // Group by SensorId
                { "latestRecord", new BsonDocument("$first", "$$ROOT") }  
            }),
            new BsonDocument("$replaceRoot", new BsonDocument("newRoot", "$latestRecord")) 
        };
    
    var result = await _sensorDataCollection.Aggregate<SensorData>(pipeline).ToListAsync();
    return result;
    }

    public async Task<byte[]> ExportToFile(ExportFormat format, PipelineDefinition<SensorData, SensorData>? pipeline){
        List<SensorData> data;

        if(pipeline == null)
             data = await GetAsync();
        else
             data = await GetAsync(pipeline);
        

        if (data == null)
            return null;

        if (format == ExportFormat.JSON)
            return Utils.ExportToJson(data, Guid.NewGuid());

        if (format == ExportFormat.CSV)
            return Utils.ExportToCsv(data, Guid.NewGuid());

        return null;

    }

    
}

