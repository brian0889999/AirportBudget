using MyGisMIS.Server.Enums;

namespace AirportBudget.Server.ViewModels
{
    public class Money
    {
        public int? ID { get; set; }
        public string? Budget { get; set; } = string.Empty;
        public string? Group { get; set; } = string.Empty;
        public string? Subject6 { get; set; } = string.Empty;
        public string? Subject7 { get; set; } = string.Empty;
        public string? Subject8 { get; set; } = string.Empty;
        public decimal? BudgetYear { get; set; }
        public string? Final { get; set; } = string.Empty;
        public int? Year { get; set; }
    }

    public class SoftDeleteViewModel
    {
        public int? ID { get; set; }
        public DateTime? Purchasedate { get; set; }
        //public FolderType? Text { get; set; }
        public string? Text { get; set; } = string.Empty;
        public string? Note { get; set; } = string.Empty;
        public int? PurchaseMoney { get; set; }
        public DateTime? PayDate { get; set; }
        public int? PayMoney { get; set; }
        public string? People { get; set; } = string.Empty;
        public string? Name { get; set; } = string.Empty;
        public string? Remarks { get; set; } = string.Empty;
        public string? People1 { get; set; } = string.Empty;
        public int ID1 { get; set; }
        public string? Status { get; set; } = string.Empty;
        public string? Group1 { get; set; } = string.Empty;
        public string? All { get; set; } = string.Empty;
        public string? True { get; set; } = string.Empty;
        public int? Year { get; set; }
        public string? Year1 { get; set; } = string.Empty;
        public Money? Money { get; set; }
        //public string? FormattedPurchaseDate { get; set; }
        //public string? FormattedPayDate { get; set; }
    }
}
