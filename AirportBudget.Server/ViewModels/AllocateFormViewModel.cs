namespace AirportBudget.Server.ViewModels
{
    public class AllocateFormViewModel
    {
        public int BudgetAmountId { get; set; } = 0;
        public string Status { get; set; } = string.Empty;
        public int BudgetId { get; set; } = 0;
        public string BudgetName { get; set; } = string.Empty;
        public int GroupId { get; set; } = 0;
        public int? InGroupId { get; set; } = 0;
        public string? GroupName { get; set; } = string.Empty;
        public string Subject6 { get; set; } = string.Empty;
        public string Subject7 { get; set; } = string.Empty;
        public string Subject8 { get; set; } = string.Empty;
        public decimal RequestAmount { get; set; } = 0;
        public decimal PaymentAmount { get; set; } = 0;
        public string? Subject6_1 { get; set; } = string.Empty;
        public string? Subject7_1 { get; set; } = string.Empty;
        public string? Subject8_1 { get; set; } = string.Empty;
        public string? RequestPerson { get; set; } = string.Empty;
        public int CreatedYear { get; set; } = DateTime.Now.Year;
        public int AmountYear { get; set; } = DateTime.Now.Year;
        public DateTime? RequestDate { get; set; } = null;
        public string? Description { get; set; } = string.Empty;
        public string? PaymentPerson { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public int Type { get; set; } = 0;
        public bool ExTax { get; set; } = false;
        public bool Reconciled { get; set; } = false;
        public bool IsValid { get; set; } = true;
    }
}
