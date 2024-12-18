using Api.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class SensorsController : ControllerBase {
    private readonly ILogger<SensorsController> _logger;
    private readonly ISensorDataService _sensorsService;
    private readonly IWalletService _walletService;
    private readonly IBlockchainService _blockchainService;
    public SensorsController(ILogger<SensorsController> logger, ISensorDataService sensorsService, IWalletService walletService, IBlockchainService blockchainService)
    {
        _logger = logger;
        _sensorsService = sensorsService;
        _walletService = walletService;
        _blockchainService = blockchainService;
    }

    [HttpGet(Name = "GetSensorsData")]
    public async Task<List<SensorData>> Get([FromQuery] int sensorId = -1,
                                            [FromQuery] string type = "",
                                            [FromQuery] DateTime startDate = default,
                                            [FromQuery] DateTime endDate = default,
                                            [FromQuery] string? sortFields = null)
    {
        List<SensorData>? sensorData;

        var sortRules = ParseSortRules(sortFields);

        if (sensorId != -1 || type != "" || startDate != default || endDate != default || (sortRules?.Count > 0))
        {
            sensorData = await _sensorsService.GetAsync(
                DbPipelineBuilder.BuildFromRequest(new SensorDataFilterOptions
                {
                    SensorId = sensorId,
                    Type = type,
                    StartDate = startDate,
                    EndDate = endDate,
                    SortRules = sortRules
                })
            );
        }
        else
        {
            sensorData = await _sensorsService.GetAsync();
        }

        return sensorData;
    }

    // Helper method to parse sorting rules from query parameters
    private List<(SortField Field, SortType Order)> ParseSortRules(string? sortFields)
    {
        var sortRules = new List<(SortField, SortType)>();

        if (!string.IsNullOrWhiteSpace(sortFields))
        {
            var fields = sortFields.Split(',');

            foreach (var field in fields)
            {
                var parts = field.Split(':');
                if (parts.Length == 2)
                {
                    if (Enum.TryParse<SortField>(parts[0], true, out var sortField) &&
                        Enum.TryParse<SortType>(parts[1], true, out var sortOrder))
                    {
                        sortRules.Add((sortField, sortOrder));
                    }
                }
            }
        }

        return sortRules;
    }

    [HttpGet("latest", Name = "GetLatestData")]
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
                                            [FromQuery] string? sortFields = null)
    {
        if (exportFormat == null)
            return BadRequest("Invalid File type");

        var sortRules = ParseSortRules(sortFields);

        if (sensorId == -1 && type == "" && startDate == default && endDate == default && (sortRules?.Count == 0))
        {
            return File(_sensorsService.ExportToFile(exportFormat).Result
                        , "application/octet-stream"
                        , $"{DateTime.UtcNow}.{(exportFormat == ExportFormat.CSV ? "csv" : "json")}");
        }

        return File(_sensorsService.ExportToFile(exportFormat,
                        DbPipelineBuilder.BuildFromRequest(new SensorDataFilterOptions
                        {
                            SensorId = sensorId,
                            Type = type,
                            StartDate = startDate,
                            EndDate = endDate,
                            SortRules = sortRules
                        }
                )).Result
                    , "application/octet-stream"
                    , $"{DateTime.UtcNow}.{(exportFormat == ExportFormat.CSV ? "csv" : "json")}");
    }

    [HttpGet("wallet/{id}")]
    async public Task<IActionResult> GetSensorWalletBalanceAsync(int id)
    {
        var balance = await _blockchainService.GetBalanceAsync(
                                                _walletService.GetSensorWalletAddress(id));
        return Ok((float)balance);
    }

    [HttpGet("wallet")]
    async public Task<IActionResult> GetSensorsWalletBalanceAsync()
    {
        var balances = await _blockchainService.GetBalanceAsync();
        return Ok(balances);
    }


}