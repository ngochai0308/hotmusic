using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotMusic.DataModel
{
    [Table("User")]
    public class Users
    {
        [Key]
        public int UserId { get; set; }
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
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiledBy { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
