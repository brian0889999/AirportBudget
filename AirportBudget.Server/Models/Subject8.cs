using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace AirportBudget.Server.Models
{
    public class Subject8
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Subject8Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Subject8Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Subject8SerialCode { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Subject8FullSerialCode { get; set; } = string.Empty;

        [ForeignKey("GroupId")]
        public int GroupId { get; set; }

        public Group? Group { get; set; }
    }
}
