using AirportBudget.Server.Enums;

namespace AirportBudget.Server.ViewModels
{
    public class BudgetAmountExcelViewModel
    {
        public int BudgetId { get; set; }
        public string BudgetName { get; set; } = string.Empty;
        public int GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public string Subject6 { get; set; } = string.Empty;
        public string Subject7 { get; set; } = string.Empty;
        public string? Subject8 { get; set; } = string.Empty;
        public int AnnualBudgetAmount { get; set; }
        public int FinalBudgetAmount { get; set; }
        public int Type { get; set; }
        public int RequestAmount { get; set; }
        public int PaymentAmount { get; set; }
        public DateTime RequestDate { get; set; }
        public int General { get; set; }
        public int Out { get; set; }
        public int In { get; set; }
        public int InActual { get; set; }
        public int UseBudget { get; set; }
        public int End { get; set; }
        public int InBalance { get; set; }
        public int SubjectActual { get; set; }
        public int Year { get; set; } // 可為null的年份
    }

    
}
