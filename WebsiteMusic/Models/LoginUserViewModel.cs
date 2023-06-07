using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebsiteMusic.Models
{
    public class LoginUserViewModel
    {
        [Required(ErrorMessage ="Tên đăng nhập không được để trống")] 
        public string UserName { get; set; }
        [Required(ErrorMessage ="Mật khẩu không được để trống")] 
        public string Password { get; set; }
        [DisplayName("Ghi nhớ tôi")]
        public bool IsRememberMe { get; set; }
    }
}
