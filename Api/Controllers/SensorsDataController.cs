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
    public async Task<List<SensorData>> Get([FromQuery] int sensorId=-1, [FromQuery] string type="", [FromQuery] DateTime startDate=default, [FromQuery] DateTime endDate=default)
    {
        List<SensorData>? sensorData;
        if(sensorId != -1 || type != "" || startDate != default || endDate != default)
        {
            sensorData = await _sensorsService.GetAsync(
                MongoDbFilterFactory.BuildFromRequest(new SensorDataFilterOptions
                {
                    SensorId = sensorId,
                    Type = type,
                    StartDate = startDate,
                    EndDate = endDate
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
    public IActionResult ExportData([FromQuery] ExportFormat exportFormat)
    {
        if (exportFormat == null)
            return BadRequest("Invalid File type");


        return File(_sensorsService
                        .ExportToFile(exportFormat).Result
                    ,"application/octet-stream"
                    , $"{DateTime.UtcNow}.{(exportFormat == ExportFormat.CSV ? "csv" : "json")}");
    


    }
}