using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteMusic.DataModel
{
    [Table("Song")]
    public class Song
    {
        [Key]
        public int SongId { get; set; }
        public string SongTitle { get; set; }
        public int CategoryId { get; set; }
        public int ArtistId { get; set; }
        public int ViewCount { get; set; }
        public string SongUrl { get; set; }
        public string Image { get; set; }

    }
}
