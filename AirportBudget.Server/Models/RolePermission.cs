using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace AirportBudget.Server.Models
{
    public class RolePermission
    {
        [Key]
        public int RolePermissionId { get; set; }

        public int PermissionType { get; set; }
        [JsonIgnore]
        public ICollection<User>? Users { get; set; }
    }
}
