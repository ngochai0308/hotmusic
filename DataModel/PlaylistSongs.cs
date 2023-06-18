using System.ComponentModel.DataAnnotations.Schema;

namespace HotMusic.DataModel
{
    [Table("PlaylistSong")]
    public class PlaylistSongs
    {
        public int PlaylistId { get; set; }
        public int SongId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiledBy { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
