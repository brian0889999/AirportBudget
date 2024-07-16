using System.ComponentModel.DataAnnotations;

namespace AirportBudget.Server.ViewModels
{
    public class DeletedRecordsViewModel
    {
        public int? ID { get; set; }
        public DateTime? Purchasedate { get; set; }
        public string? Text { get; set; } = string.Empty;
        public string? Note { get; set; } = string.Empty;
        public decimal? PurchaseMoney { get; set; }
        public DateTime? PayDate { get; set; }
        public decimal? PayMoney { get; set; }
        public string? People { get; set; } = string.Empty;
        public string? Name { get; set; } = string.Empty;
        public string? Remarks { get; set; } = string.Empty;
        public string? People1 { get; set; } = string.Empty;
        public int? ID1 { get; set; }
        public string? Status { get; set; } = string.Empty;
        public string? Group1 { get; set; } = string.Empty;
        public string? All { get; set; } = string.Empty;
        public string? True { get; set; } = string.Empty;
        public int? Year { get; set; }
        public string? Year1 { get; set; } = string.Empty;
    }
}
