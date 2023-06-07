using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteMusic.DataModel
{
    [Table("PlaylistSong")]
    public class PlaylistSong
    {
        public int PlaylistId { get; set; }
        public int SongId { get; set; }
    }
}
