using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteMusic.DataModel
{
    [Table("Playlist")]
    public class Playlist
    {
        [Key]
        public int PlaylistId { get; set; }
        public string PlaylistTitle { get; set; }
        public string UserId { get; set; }
    }
}
