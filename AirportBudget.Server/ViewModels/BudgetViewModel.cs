namespace AirportBudget.Server.ViewModels
{
    public class BudgetViewModel
    {
        public int BudgetId { get; set; }
        public string BudgetName { get; set; } = string.Empty;
        public string Subject6 { get; set; } = string.Empty;
        public string Subject7 { get; set; } = string.Empty;
        public string Subject8 { get; set; } = string.Empty;
        public int AnnualBudgetAmount { get; set; }
        public int FinalBudgetAmount { get; set; }
        public int CreatedYear { get; set; }
        public int GroupId { get; set; }

        public GroupViewModel? Group { get; set; }
    }
}
