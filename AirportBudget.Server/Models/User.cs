using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AirportBudget.Server.Enums;
using System.Text.RegularExpressions;

namespace AirportBudget.Server.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        
        [StringLength(10)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Account { get; set; } = string.Empty;

        [Required]
        [StringLength(256)]
        public string Password { get; set; } = string.Empty;

        [ForeignKey("RolePermissionId")]
        [Required]
        public int RolePermissionId { get; set; }

        [ForeignKey("GroupId")]
        public int GroupId { get; set; }

        public bool Status { get; set; }

        public UserSystemType? System { get; set; }

        public DateTime? LastPasswordChangeDate { get; set; }

        public int ErrCount { get; set; } = 0;

        public DateTime ErrDate { get; set; } = new DateTime(1990, 1, 1);

        public Group? Group { get; set; }
        public RolePermission? RolePermission { get; set; }
    }
}
