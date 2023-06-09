using System.ComponentModel.DataAnnotations;

namespace HotMusic.Models
{
    public class SongViewModel
    {
        [Display(Name = "Mã bài hát")]
        public int SongId { get; set; }

        [Display(Name = "Tên bài hát")]
        public string SongTitle { get; set; }

        [Display(Name = "Mã nghệ sĩ")]
        public int ArtistId { get; set; }

        [Display(Name = "Mã Album")]
        public int AlbumId { get; set; }

        [Display(Name = "Lượt nghe")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int ViewCount { get; set; }

        [Display(Name = "Link")]
        public string SongUrl { get; set; }

        [Display(Name = "Tên nghệ sĩ")]
        [Required(AllowEmptyStrings = true)]
        public string ArtistName { get; set; } = string.Empty;

        [Display(Name = "Tên Album")]
        [Required(AllowEmptyStrings = true)]
        public string AlbumName { get; set; } = string.Empty;
    }
}
