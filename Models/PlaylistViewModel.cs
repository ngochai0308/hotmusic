using System.ComponentModel.DataAnnotations;

namespace HotMusic.Models
{
    public class PlaylistViewModel
    {
        [Display(Name = "Mã danh sách")]
        public int PlaylistId { get; set; }

        [Display(Name = "Tên danh sách")]
        public string PlaylistTitle { get; set; }

        [Display(Name = "Mã người dùng")]
        public int UserId { get; set; }

        [Display(Name = "Tài khoản")]
        public string? UserName { get; set; }
    }
}
