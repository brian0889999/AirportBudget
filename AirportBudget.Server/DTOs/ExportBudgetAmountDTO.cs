using AirportBudget.Server.Models;

namespace AirportBudget.Server.DTOs;

public class ExportBudgetAmountDTO
{
    public string Subject6 { get; set; } = string.Empty;
    public string Subject7 { get; set; } = string.Empty;
    public string Subject8 { get; set; } = string.Empty;
    public int AnnualBudgetAmount { get; set; }
    public int FinalBudgetAmount { get; set; }
    public int General { get; set; }
    public int Out { get; set; }
    public int UseBudget { get; set; }
    public int In { get; set; }
    public int InActual { get; set; }
    public int InBalance { get; set; }
    public int SubjectActual { get; set; }
    public int InUseBudget { get; set; }
    public int END { get; set; }
}
