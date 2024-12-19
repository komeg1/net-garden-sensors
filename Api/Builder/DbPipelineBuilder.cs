using Api.Entities.Enums;
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

        if (options.SortRules != null && options.SortRules.Count > 0)
        {
            var sortDefinitions = new List<SortDefinition<SensorData>>();

            foreach (var rule in options.SortRules)
            {
                switch (rule.Field)
                {
                    case SortField.SensorId:
                        sortDefinitions.Add(rule.Order == SortType.ASCENDING
                            ? sort.Ascending(e => e.SensorId)
                            : sort.Descending(e => e.SensorId));
                        break;

                    case SortField.Unit:
                        sortDefinitions.Add(rule.Order == SortType.ASCENDING
                            ? sort.Ascending(e => e.Unit)
                            : sort.Descending(e => e.Unit));
                        break;

                    case SortField.Timestamp:
                        sortDefinitions.Add(rule.Order == SortType.ASCENDING
                            ? sort.Ascending(e => e.Timestamp)
                            : sort.Descending(e => e.Timestamp));
                        break;
                    case SortField.Value:
                        sortDefinitions.Add(rule.Order == SortType.ASCENDING
                            ? sort.Ascending(e => e.Value)
                            : sort.Descending(e => e.Value));
                        break;
                }
            }

            // Combine multiple sort definitions
            pipeline = pipeline.Sort(sort.Combine(sortDefinitions));
        }

        return pipeline;


    }

}