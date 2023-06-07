using System.ComponentModel.DataAnnotations;

namespace WebsiteMusic.Models
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(4,ErrorMessage = "Mật khẩu phải tối thiểu 4 kí tự")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Nhập lại mật khẩu không được để trống")]
        [Compare("Password",ErrorMessage ="Mật khẩu nhập lại phải giống mật khẩu")]
        public string ConfilmPassword { get; set; }
    }
}
