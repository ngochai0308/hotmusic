using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotMusic.DataModel
{
    [Table("Album")]
    public class Albums
    {
        [Key]
        public int AlbumId { get; set; }
        public string AlbumTitle { get; set; }
        public string? Thumbnail { get; set; }
        public int ArtistId { get; set; }
        public int CategoryID { get; set; }
        public DateTime? CreatedDate {  get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiledBy { get; set; }
        public bool? IsDeleted { get; set; }

    }
}
