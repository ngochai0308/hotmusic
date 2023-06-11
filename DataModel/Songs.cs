using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotMusic.DataModel
{
    [Table("Songs")]
    public class Songs
    {
        [Key]
        public int SongId { get; set; }

        public string SongTitle { get; set; }

        public int ArtistId { get; set; }

        public int AlbumId { get; set; }

        public int ViewCount { get; set; }

        public string SongUrl { get; set; }

        [NotMapped]
        [Required(AllowEmptyStrings = true)]
        public string ArtistName { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = true)]
        [NotMapped]
        public string AlbumName { get; set; } = string.Empty;
    }
}
