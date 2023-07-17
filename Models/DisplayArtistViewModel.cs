using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HotMusic.Models
{
    public class DisplayArtistViewModel
    {
        public int ArtistId { get; set; }
        [DisplayName("Tên nghệ sĩ")]
        public string ArtistName { get; set; }
        [DataType(DataType.Upload)]
        [DisplayName("Chọn ảnh nghệ sĩ")]
        public IFormFile? FileUpload { get; set; }
        [DisplayName("Tiểu sử")]
        public string ArtistBio { get; set; }
        [DisplayName("Ảnh đại diện")]
        public string? Avatar { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiledBy { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
