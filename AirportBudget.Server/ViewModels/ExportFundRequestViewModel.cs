using AirportBudget.Server.Models;

namespace AirportBudget.Server.ViewModels;

public class ExportFundRequestViewModel
{
    public int Year { get; set; }
    public int StartMonth { get; set; }
    public int EndMonth { get; set; }
}
