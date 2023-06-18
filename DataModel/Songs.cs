using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotMusic.DataModel
{
    [Table("Song")]
    public class Songs
    {
        [Key]
        public int SongId { get; set; }
        public string SongTitle { get; set; }
        public int CategoryId { get; set; }
        public int ArtistId { get; set; }
        public int ViewCount { get; set; }
        public string SongUrl { get; set; }
        public string Image { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiledBy { get; set; }
        public bool? IsDeleted { get; set; }
        [NotMapped]
        public string CategoryTitle { get; set; }
        [NotMapped]
        public string ArtistName { get; set; }

    }
}
