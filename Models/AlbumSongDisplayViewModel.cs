using System.ComponentModel;

namespace HotMusic.Models
{
    public class AlbumSongDisplayViewModel
    {
<<<<<<< HEAD
        public int? Id { get; set; }
=======
>>>>>>> 1f9e9255cc503a3619dba5c2a37cf3fefe559c4c
        [DisplayName("Mã Album")]
        public int AlbumId { get; set; }
        [DisplayName("Mã bài hát")]
        public int SongId { get; set; }
        [DisplayName("Tên Album")]
        public string? AlbumTitle { get; set; }
        [DisplayName("Tên bài hát")]
        public string? SongTitle { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiledBy { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
