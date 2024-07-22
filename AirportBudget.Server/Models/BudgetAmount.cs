using AirportBudget.Server.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace AirportBudget.Server.Models
{
    public class BudgetAmount
    {
        [Key]
        public int BudgetAmountId { get; set; }

        [StringLength(255)]
        public string Description { get; set; } = string.Empty;

        public AmountType Type { get; set; }

        public int RequestAmount { get; set; }

        public int PaymentAmount { get; set; }

        public DateTime? RequestDate { get; set; }

        public DateTime? PaymentDate { get; set; }

        [StringLength(100)]
        public string RequestPerson { get; set; } = string.Empty;

        [StringLength(100)]
        public string PaymentPerson { get; set; } = string.Empty;

        public bool ExTax { get; set; }

        public bool Reconciled { get; set; }

        [StringLength(255)]
        public string Remarks { get; set; } = string.Empty;

        [StringLength(50)]
        public string Status { get; set; } = string.Empty;

        public int CreatedYear { get; set; }

        public int AmountYear { get; set; }

        [ForeignKey("BudgetId")]
        public int BudgetId { get; set; }

        

        //public int AmountSerialNumber { get; set; }
        public bool IsValid { get; set; }
        public int? LinkedBudgetAmountId { get; set; }

        public  Budget? Budget { get; set; }
    }
}
