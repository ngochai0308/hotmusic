using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HotMusic.Models
{
    public class SongViewModel
    {
        [DisplayName("Mã bài hát")]
        public int SongId { get; set; }
        [DisplayName("Tên bài hát")]
        public string SongTitle { get; set; }
        [DisplayName("Mã thế loại")]
        public int CategoryId { get; set; }
        [DisplayName("Mã nghệ sĩ")]
        public int ArtistId { get; set; }
        [DisplayName("Lượt nghe")]
        public int ViewCount { get; set; }
        public string SongUrl { get; set; }

        [DataType(DataType.Upload)]
        [DisplayName("Chọn ảnh bài hát")]
        public IFormFile? FileUpload { get; set; }

        [DisplayName("Thể loại")]
        public string? CategoryTitle { get; set; }
        [DisplayName("Nghệ sĩ")]
        public string? ArtistName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiledBy { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
