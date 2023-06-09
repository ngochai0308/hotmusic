using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhac.DataModel
{
    [Table("Albums")]
    public class Albums
    {
        [Key]
        public int AlbumId { get; set; }
        public string AlbumTitle { get; set; }
        public int ArtistId { get; set; }
        [NotMapped]
        public string ArtistName { get; set; }
    }
}
