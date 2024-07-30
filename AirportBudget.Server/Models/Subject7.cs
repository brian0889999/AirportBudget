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

        [MaxLength(100)]
        public string Subject7FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Subject7SerialCode { get; set; } = string.Empty;

        [ForeignKey("GroupId")]
        public int GroupId { get; set; }

        public Group? Group { get; set; }
    }
}
