using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebsiteMusic.Models
{
    public class EditUserViewModel
    {
        public string UserId { get; set; }
        public string? UserName { get; set; }
        public string Password { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }

        [DataType(DataType.Upload)]
        [DisplayName("Chọn file upload")]

        public IFormFile? FileUpload { get; set; }
    }
}
