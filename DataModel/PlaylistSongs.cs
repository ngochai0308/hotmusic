using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhac.DataModel
{
    [Table("PlaylistSong")]
    public class PlaylistSongs
    {
        //[Key] => Không cần định nghĩa key cho cacs table có >1 cột làm khóa ở đây
        public int SongId { get; set; }

        public int PlaylistId { get; set; }
    }
}
