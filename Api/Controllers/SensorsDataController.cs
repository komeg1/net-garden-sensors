using Microsoft.AspNetCore.Mvc;
namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class SensorsController : ControllerBase{
    private readonly ILogger<SensorsController> _logger;
    private readonly ISensorDataService _sensorsService;
    public SensorsController(ILogger<SensorsController> logger, ISensorDataService sensorsService)
    {
        _logger = logger;
        _sensorsService = sensorsService;
    }

    [HttpGet(Name = "GetSensorsData")]
    public async Task<List<SensorData>> Get([FromQuery] int sensorId=-1,
                                            [FromQuery] string type="",
                                            [FromQuery] DateTime startDate=default,
                                            [FromQuery] DateTime endDate=default,
                                            [FromQuery] SortType sort = SortType.NONE)
    {
        List<SensorData>? sensorData;
        if(sensorId != -1 || type != "" || startDate != default || endDate != default || sort != SortType.NONE)
        {
            sensorData = await _sensorsService.GetAsync(
                DbPipelineBuilder.BuildFromRequest(new SensorDataFilterOptions
                {
                    SensorId = sensorId,
                    Type = type,
                    StartDate = startDate,
                    EndDate = endDate,
                    Sort = sort
                })
            );
        }
        else{
            sensorData = await _sensorsService.GetAsync();
        }

    return sensorData;
        
    }

    [HttpGet("latest",Name="GetLatestData")]
    public async Task<List<SensorData>> GetLatest()
    {
        var sensorData = await _sensorsService.GetNewestDataAsync();
        return sensorData;
        
    }

    [HttpGet("export")]
    public IActionResult ExportData([FromQuery] ExportFormat exportFormat,[FromQuery] int sensorId=-1,
                                            [FromQuery] string type="",
                                            [FromQuery] DateTime startDate=default,
                                            [FromQuery] DateTime endDate=default,
                                            [FromQuery] SortType sort = SortType.NONE)
    {
        if (exportFormat == null)
            return BadRequest("Invalid File type");

        if(sensorId == -1 && type == "" && startDate == default && endDate == default && sort ==SortType.NONE)
        {
            return File(_sensorsService
                        .ExportToFile(exportFormat).Result
                    ,"application/octet-stream"
                    , $"{DateTime.UtcNow}.{(exportFormat == ExportFormat.CSV ? "csv" : "json")}");
        }

        return File(_sensorsService
                    .ExportToFile(exportFormat,
                        DbPipelineBuilder.BuildFromRequest(new SensorDataFilterOptions
                        {
                            SensorId = sensorId,
                            Type = type,
                            StartDate = startDate,
                            EndDate = endDate,
                            Sort = sort
                        }
                )).Result
                    ,"application/octet-stream"
                    , $"{DateTime.UtcNow}.{(exportFormat == ExportFormat.CSV ? "csv" : "json")}");

        
    


    }
}