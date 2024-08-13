using AirportBudget.Server.Enums;
using AirportBudget.Server.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirportBudget.Server.ViewModels
{
    public class UserViewModel
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Account { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int RolePermissionId { get; set; }
        public int GroupId { get; set; }
        public bool Status { get; set; }
        public UserSystemType? System { get; set; }

        public DateTime? LastPasswordChangeDate { get; set; }

        public int ErrCount { get; set; }
        public DateTime ErrDate { get; set; }

        public GroupViewModel? Group { get; set; }
        public RolePermissionViewModel? RolePermission { get; set; }
    }
}
