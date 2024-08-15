using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace AirportBudget.Server.Models
{
    public class Group
    {
        [Key]
        public int GroupId { get; set; }

        [Required]
        [StringLength(50)]
        public string GroupName { get; set; } = string.Empty;


        public ICollection<User>? Users { get; set; }

        [JsonIgnore]
        public ICollection<Budget>? Budgets { get; set; }


        public ICollection<Subject6>? Subject6s { get; set; }


        //public ICollection<Subject7>? Subject7s { get; set; }

        //public ICollection<Subject8>? Subject8s { get; set; }
    }
}
