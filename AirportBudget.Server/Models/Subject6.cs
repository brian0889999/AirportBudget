//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Text.RegularExpressions;

//namespace AirportBudget.Server.Models
//{
//    public class Subject6
//    {
//        [Key]
//        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//        public int Subject6Id { get; set; }

//        [Required]
//        [MaxLength(100)]
//        public string Subject6Name { get; set; } = string.Empty;

//        [ForeignKey("GroupId")]
//        public int GroupId { get; set; }

//        public Group? Group { get; set; }

//        public ICollection<Subject7>? subject7s { get; set; }
//    }
//}
