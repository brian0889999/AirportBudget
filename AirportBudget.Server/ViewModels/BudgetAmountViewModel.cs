namespace AirportBudget.Server.ViewModels
{
    public class BudgetAmountViewModel
    {
        public int BudgetAmountId { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Type { get; set; }
        public int RequestAmount { get; set; }
        public int PaymentAmount { get; set; }
        public DateTime? RequestDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string RequestPerson { get; set; } = string.Empty;
        public string PaymentPerson { get; set; } = string.Empty;
        public bool ExTax { get; set; }
        public bool Reconciled { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int CreatedYear { get; set; }
        public int AmountYear { get; set; }
        public int BudgetId { get; set; }
        public int AmountSerialNumber { get; set; }
        public bool IsValid { get; set; }

        public BudgetViewModel? Budget { get; set; }
    }

    //public class BudgetViewModel
    //{
    //    public int BudgetId { get; set; }
    //    public string BudgetName { get; set; } = string.Empty;
    //    public string Subject6 { get; set; } = string.Empty;
    //    public string Subject7 { get; set; } = string.Empty;
    //    public string Subject8 { get; set; } = string.Empty;
    //    public int AnnualBudgetAmount { get; set; }
    //    public int FinalBudgetAmount { get; set; }
    //    public int CreatedYear { get; set; }
    //    public int GroupId { get; set; }

    //    public GroupViewModel? Group { get; set; }
    //}

    //public class GroupViewModel
    //{
    //    public int GroupId { get; set; }
    //    public string GroupName { get; set; } = string.Empty;
    //}
}
