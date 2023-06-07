using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteMusic.DataModel
{
    [Table("Artist")]
    public class Artist
    {
        [Key]
        public int ArtistId { get; set; }
        public string ArtistName { get; set; }
        public string ArtistBio { get; set; }
    }
}
