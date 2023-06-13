using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotMusic.DataModel
{
    [Table("Playlists")]
    public class Playlists 
    {
        [Key]
        public int PlaylistId { get; set; }
        public string PlaylistTitle { get; set; }
        public int UserId { get; set; }

        [NotMapped]
        public string UserName { get; set; }
    }
}
