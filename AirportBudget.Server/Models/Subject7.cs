using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace AirportBudget.Server.Models
{
    public class Subject7
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Subject7Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Subject7Name { get; set; } = string.Empty;



        [ForeignKey("Subject6Id")]
        public int Subject6Id { get; set; }

        public Subject6? Subject6 { get; set; }

        public ICollection<Subject8>? Subject8s { get; set; }
    }
}
