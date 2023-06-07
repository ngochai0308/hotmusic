using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteMusic.DataModel
{
    [Table("Favourite")]
    public class Favourite
    {
        public int SongId { get; set; }
        public string UserId { get; set; }
    }
}
