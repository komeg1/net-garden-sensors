using Microsoft.AspNetCore.Mvc.Filters;
using MongoDB.Driver;

namespace Api;
public static class DbPipelineBuilder{

    public static PipelineDefinition<SensorData,SensorData> BuildFromRequest(SensorDataFilterOptions options)
    {
        var filter = Builders<SensorData>.Filter;
        var sort = Builders<SensorData>.Sort;
        PipelineDefinition<SensorData,SensorData> pipeline = new EmptyPipelineDefinition<SensorData>();

        if (options.SensorId >=0){pipeline =pipeline.Match(filter.Eq(e=> e.SensorId, options.SensorId));}
        if (options.Type != "") {pipeline = pipeline.Match(filter.Eq(e=> e.Unit, options.Type));}
        if (options.StartDate != default) {pipeline = pipeline.Match(filter.Gte(e=> e.Timestamp, options.StartDate));}
        if (options.EndDate != default) {pipeline = pipeline.Match(filter.Lte(e=> e.Timestamp, options.EndDate));}

        if(options.Sort == SortType.ASCENDING)
                pipeline= pipeline.Sort(sort.Ascending(e=>e.Timestamp));
         if(options.Sort == SortType.DESCENDING)
                 pipeline = pipeline.Sort(sort.Descending(e=>e.Timestamp));
        
        return pipeline;

    }

}