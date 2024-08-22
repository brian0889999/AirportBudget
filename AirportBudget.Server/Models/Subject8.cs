//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Text.RegularExpressions;

//namespace AirportBudget.Server.Models
//{
//    public class Subject8
//    {
//        [Key]
//        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//        public int Subject8Id { get; set; }

//        [Required]
//        [MaxLength(100)]
//        public string Subject8Name { get; set; } = string.Empty;

//        [ForeignKey("Subject7Id")]
//        public int Subject7Id { get; set; }

//        public Subject7? Subject7 { get; set; }
//    }
//}
