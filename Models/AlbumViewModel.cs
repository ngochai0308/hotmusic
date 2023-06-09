using System.ComponentModel.DataAnnotations;

namespace HotMusic.Models
{
    public class AlbumViewModel
    {
        [Display(Name = "Mã Album")]
        public int AlbumId { get; set; }

        [Display(Name = "Tên Album")]
        [Required(ErrorMessage = "Vui lòng nhập tên Album")]
        public string AlbumTitle { get; set; }

        [Display(Name = "Mã nghệ sĩ")]
        public int ArtistId { get; set; }

        [Display(Name = "Nghệ sĩ")]
        public string? ArtistName { get; set;}
    }
}
