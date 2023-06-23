using System.ComponentModel.DataAnnotations;

namespace HotMusic.Models
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Mật khẩu không được để trống!")]
        public string Password { get; set; }


        [Required(ErrorMessage = "Mật khẩu mới không được để trống!")]
        [MinLength(4,ErrorMessage ="Mật khẩu phải lớn hơn 4 kí tự")] public string NewPassword { get; set; }


        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống!")]
        [Compare("NewPassword", ErrorMessage = "Xác nhận mật khẩu và mật khẩu mới phải giống nhau")]
        public string ConfirmPassword { get; set; }
    }
}
