using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HotMusic.Models
{
    public class PlaylistSongViewModel
    {
        [Display(Name = "Mã bài hát")]
        public int SongId { get; set; }

        [Display(Name = "Mã danh sách")]
        public int PlaylistId { get; set; }

        [Display(Name = "Tên bài hát")]
        [Required(AllowEmptyStrings = true)]
        [NotMapped]
        public string SongName { get; set; } = string.Empty;

        [Display(Name = "Tên danh sách")]
        [NotMapped]
        [Required(AllowEmptyStrings = true)]
        public string PlaylistName { get; set; } = string.Empty;
    }
}
