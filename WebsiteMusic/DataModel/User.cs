using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteMusic.DataModel
{
    [Table("User")]
    public class User
    {
        [Key]
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string? FullName { get; set; } 
        public string? Email { get; set; } 
        public string? Role { get; set; } 
        public string? PhoneNumber { get; set; } 
        public DateTime? DateOfBirth { get; set; }
        public string? image { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
    }
}
