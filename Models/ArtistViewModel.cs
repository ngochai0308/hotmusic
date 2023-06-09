using System.ComponentModel.DataAnnotations;

namespace HotMusic.Models
{
    public class ArtistViewModel
    {
        [Display(Name = "Mã nghệ sĩ")]
        public int ArtistId { get; set; }

        [Display(Name = "Tên nghệ sĩ")]
        [Required(ErrorMessage = "Vui lòng nhập tên nghệ sĩ")]
        public string ArtistName { get; set; }

        [Display(Name = "Tiểu sử")]
        [Required(ErrorMessage = "Vui lòng nhập tiểu sử")]
        public string? ArtistBio { get; set; }
    }
}
