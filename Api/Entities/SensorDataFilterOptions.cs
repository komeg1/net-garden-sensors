using Api.Entities.Enums;

public class SensorDataFilterOptions{
    public int SensorId {get;set;}
    public string Type {get;set;} = null!;
    public DateTime StartDate {get;set;}
    public DateTime EndDate {get;set;}
    public SortType Sort {get;set;} = SortType.NONE;

    public List<(SortField Field, SortType Order)> SortRules { get; set; } = new();
}