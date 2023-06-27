using System.ComponentModel;

namespace HotMusic.Models
{
    public class LyricDisplayViewModel
    {
        public int Id { get; set; }
        [DisplayName("Lời bài hát")]
        public string lyricc { get; set; }
        [DisplayName("Mã bài hát")]
        public int SongId { get; set; }
        [DisplayName("Tên bài hát")]
        public string? SongTitle { get; set; }
    }
}
