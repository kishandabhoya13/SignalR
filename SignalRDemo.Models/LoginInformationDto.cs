using System.ComponentModel.DataAnnotations;

namespace SignalRDemo.Models.Models
{
    public class LoginInformationDto
    {
        [Key]
        public int Id { get; set; }

        public int StudentId { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; } = null!;

        [StringLength(50)]
        public string LastName { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public string Email { get; set; }

        public int RoleId { get; set; }

        public string? JwtToken { get; set; } = null;

        public string? SettingDescription { get; set; } = "1.0";

        public bool IsConfirmed { get; set; }

        public bool IsRejected { get; set; }

        public bool IsBlocked { get; set; }

        public string token { get; set; }

        public bool IsPasswordUpdated { get; set; } = false;
    }
}
