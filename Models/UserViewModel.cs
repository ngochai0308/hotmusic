using System.ComponentModel.DataAnnotations;

namespace HotMusic.Models
{
    public class UserViewModel
    {
        [Display(Name = "Mã người dùng")]
        public int UserId { get; set; }

        [Display(Name = "Họ và tên")]
        [Required(ErrorMessage = "Họ tên không được để trống")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(6, ErrorMessage = "Mật khẩu tối thiểu 6 kí tự")]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Tài khoản không được để trống")]
        [Display(Name = "Tài khoản")]
        public string UserName { get; set; }

        [Display(Name = "Hòm thư điện tử")]
        public string Email { get; set; }

        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [Display(Name = "Lưu thông tin đăng nhập!")]
        public bool IsRememberMe { get; set; } = false;
    }
}
