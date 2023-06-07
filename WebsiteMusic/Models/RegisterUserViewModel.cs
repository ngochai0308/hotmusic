using System.ComponentModel.DataAnnotations;

namespace WebsiteMusic.Models
{
    public class RegisterUserViewModel
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress] public string Email { get; set; }


        [Required(ErrorMessage = "Mật khẩu không được để trống")]

        [MinLength(4,ErrorMessage ="Mật khẩu tối thiểu 4 kí tự")] 
        public string Password { get; set; }

        [Required(ErrorMessage = "xác nhận mật khẩu không được để trống")]
        [Compare("Password",ErrorMessage ="Mật khẩu nhập lại phải giống mật khẩu!")] 
        public string ConfilmPassword { get; set; }

    }
}
