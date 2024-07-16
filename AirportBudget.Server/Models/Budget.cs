using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace AirportBudget.Server.Models
{
    public class Budget
    {
        [Key]
        public int BudgetId { get; set; }

        [Required]
        [StringLength(100)]
        public string BudgetName { get; set; } = string.Empty;

        [StringLength(50)]
        public string Subject6 { get; set; } = string.Empty;

        [StringLength(50)]
        public string Subject7 { get; set; } = string.Empty;

        [StringLength(50)]
        public string Subject8 { get; set; } = string.Empty;

        public int AnnualBudgetAmount { get; set; }

        public int FinalBudgetAmount { get; set; }

        public int CreatedYear { get; set; }

        [ForeignKey("GroupId")]
        public int GroupId { get; set; }

        public Group? Group { get; set; }

        [JsonIgnore]
        public ICollection<BudgetAmount>? BudgetAmounts {  get; set; } 
    }
}
