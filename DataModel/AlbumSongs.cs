using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotMusic.DataModel
{
    [Table("AlbumSong")]
    public class AlbumSongs
    {
        public int AlbumId { get; set; }
        public int SongId { get; set; }
        [NotMapped]
        public string? AlbumTitle { get; set; }
        [NotMapped]
        public string? SongTitle { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiledBy { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
