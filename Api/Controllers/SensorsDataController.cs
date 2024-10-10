using Microsoft.AspNetCore.Mvc;
namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class SensorsController : ControllerBase{
    private readonly ILogger<SensorsController> _logger;
    private readonly ISensorsService _sensorsService;
    public SensorsController(ILogger<SensorsController> logger, ISensorsService sensorsService)
    {
        _logger = logger;
        _sensorsService = sensorsService;
    }

    [HttpGet(Name = "GetSensorsData")]
    public async Task<List<SensorData>> Get()
    {
        var sensorData = await _sensorsService.GetAsync();
        return sensorData;
        
    }
}