using Microsoft.AspNetCore.Mvc.Filters;
using MongoDB.Driver;

namespace Api;
public static class MongoDbFilterFactory{

    public static FilterDefinition<SensorData> BuildFromRequest(SensorDataFilterOptions options)
    {
        var idFilterBuilder = Builders<SensorData>.Filter;
        var typeFilterBuilder= Builders<SensorData>.Filter;
        var startDateFilterBuilder= Builders<SensorData>.Filter;
        var endDateFilterBuilder= Builders<SensorData>.Filter;
       
        List<FilterDefinition<SensorData>> filters = new List<FilterDefinition<SensorData>>();
        if (options.SensorId >=0){filters.Add(idFilterBuilder.Eq(e=> e.SensorId, options.SensorId));}
        if (options.Type != "") {filters.Add(typeFilterBuilder.Eq(e=> e.Unit, options.Type));}
        if (options.StartDate != default) {filters.Add(startDateFilterBuilder.Gte(e=> e.Timestamp, options.StartDate));}
        if (options.EndDate != default) {filters.Add(endDateFilterBuilder.Lte(e=> e.Timestamp, options.EndDate));}
        
        

        return Builders<SensorData>.Filter.And(filters);
        
        

    }

}