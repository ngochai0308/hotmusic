using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [Required]
        public int ArtistId { get; set; }

        [Display(Name = "Nghệ sĩ")]
        [NotMapped]
        public string? ArtistName { get; set;}

        [Display(Name ="Ảnh Album")]
        [NotMapped]
        public string? Thumbnail { get; set; }

        [DisplayName("Mã thể loại")]
        [Required]
        public int CategoryID { get; set; }

        [DisplayName("Thể loại")]
        [NotMapped]
        public string? CategoryTitle { get; set; }

        [DataType(DataType.Upload)]
        [DisplayName("Chọn ảnh Album")]
        [Required]
        public IFormFile? FileUpload { get; set; }

        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiledBy { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
