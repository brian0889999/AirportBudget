using System.ComponentModel.DataAnnotations;

namespace AirportBudget.Server.ViewModels
{
    public class UserDataViewModel
    {
        [Key]
        public int No { get; set; }
        public string? Name { get; set; } = string.Empty;
        public string? Account { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
        public string? Auth { get; set; } = string.Empty;
        public string? Status1 { get; set; } = string.Empty;
        public string? Status2 { get; set; } = string.Empty;
        public string? Status3 { get; set; } = string.Empty;
    }
}
