using AirportBudget.Server.Models;

namespace AirportBudget.Server.DTOs;

public class ExportFundDTO
{
    public string Name { get; set; } = string.Empty;
    public string RequestPerson { get; set; } = string.Empty;
    public int Money { get; set; }    
}
